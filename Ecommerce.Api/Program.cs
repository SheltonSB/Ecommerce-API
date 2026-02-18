using Ecommerce.Api.Data;
using Ecommerce.Api.Middleware;
using Ecommerce.Api.Domain;
using Ecommerce.Api.Services;
using Ecommerce.Api.Infrastructure;
using Ecommerce.Api;
using Polly;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Threading.RateLimiting;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);

// 1. Database (Neon Postgres)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Validate critical secrets early (prod only)
if (!builder.Environment.IsDevelopment())
{
    SecretsValidator.Validate(builder.Configuration);
}

// 2. Auth (Identity)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Require a confirmed email to log in.
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// 3. JWT Configuration
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key missing in configuration.");
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // In production, this should be true.
    options.RequireHttpsMetadata = builder.Environment.IsDevelopment() ? false : true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"]
    };
});

// 4. CORS (Allow Frontend)
builder.Services.AddCors(options =>
{
    var frontendURL = builder.Configuration.GetValue<string>("FrontendURL");
    var vercelUrl = builder.Configuration["VercelUrl"] ?? builder.Configuration["VERCEL_URL"];

    var allowedOrigins = new List<string>();
    void AddOriginIfValid(string? rawOrigin)
    {
        if (string.IsNullOrWhiteSpace(rawOrigin))
        {
            return;
        }

        if (rawOrigin.Contains("your-production-domain.com", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var normalized = rawOrigin.StartsWith("http", StringComparison.OrdinalIgnoreCase)
            ? rawOrigin
            : $"https://{rawOrigin}";

        if (!Uri.TryCreate(normalized, UriKind.Absolute, out var parsed) ||
            (parsed.Scheme != Uri.UriSchemeHttp && parsed.Scheme != Uri.UriSchemeHttps))
        {
            return;
        }

        allowedOrigins.Add($"{parsed.Scheme}://{parsed.Authority}");
    }

    AddOriginIfValid(frontendURL);
    AddOriginIfValid(vercelUrl);

    if (builder.Environment.IsDevelopment())
    {
        AddOriginIfValid("http://localhost:5173");
        AddOriginIfValid("https://localhost:5173");
        AddOriginIfValid("http://127.0.0.1:5173");
    }

    allowedOrigins = allowedOrigins
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToList();

    if (allowedOrigins.Count == 0)
    {
        throw new InvalidOperationException("No valid CORS frontend origins configured.");
    }

    Console.WriteLine($"[CORS] Configured Allowed Origins: {string.Join(", ", allowedOrigins)}");

    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(allowedOrigins.ToArray())
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>(); // Register Validators

// [CRITICAL FIX] Register AutoMapper so Services can use IMapper
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "E-commerce API", Version = "v1" });

    // Configure Swagger to use JWT authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// Configure Cloudinary Settings
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));

// Register Application Services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ISaleService, SaleService>();
// Use real email sender in non-development; fall back to dummy for local dev
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IEmailService, DummyEmailService>();
}
else
{
    builder.Services.AddScoped<IEmailService, EmailSender>();
}
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>(); // Register the AI Analytics Service
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<ImageService>();

// Resilience policies (retry + circuit breaker) for outbound calls (e.g., Stripe/Cloudinary)
builder.Services.AddSingleton<IAsyncPolicy>(sp =>
{
    var retry = Policy
        .Handle<Exception>()
        .WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds(150 * attempt));

    var circuit = Policy
        .Handle<Exception>()
        .CircuitBreakerAsync(2, TimeSpan.FromSeconds(20));

    return Policy.WrapAsync(retry, circuit);
});

// 5. Caching (Redis)
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
if (!string.IsNullOrEmpty(redisConnectionString) && builder.Environment.IsProduction())
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConnectionString;
        options.InstanceName = "ecommerce_";
    });
    Console.WriteLine("[INFO] Using Redis for distributed caching.");
}
else
{
    builder.Services.AddDistributedMemoryCache();
    Console.WriteLine("[INFO] Using in-memory cache. Set 'ConnectionStrings:Redis' for distributed caching in production.");
}

// Memory cache for in-process caching and ICacheProvider wiring
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICacheProvider>(sp =>
{
    // Prefer distributed cache when configured in production; otherwise fall back to in-memory cache.
    if (!string.IsNullOrEmpty(redisConnectionString) && builder.Environment.IsProduction())
    {
        return new DistributedCacheProvider(sp.GetRequiredService<IDistributedCache>());
    }

    return new MemoryCacheProvider(sp.GetRequiredService<IMemoryCache>());
});

// Rate limiting for admin endpoints
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("authLimiter", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User?.Identity?.Name ?? context.Request.Headers.Host.ToString(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 20,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 5,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            }));
});

// OpenTelemetry tracing
builder.Services.AddOpenTelemetry().WithTracing(tracer =>
{
    tracer
        .SetResourceBuilder(OpenTelemetry.Resources.ResourceBuilder.CreateDefault().AddService("Ecommerce.Api"))
        .AddAspNetCoreInstrumentation()
        .AddEntityFrameworkCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter();
});

var app = builder.Build();

// Apply migrations automatically on startup (container-friendly)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
        Console.WriteLine("✅ Database migrated successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Migration failed: {ex.Message}");
    }
}

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// Configure Pipeline
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

var spaIndex = app.Environment.WebRootFileProvider.GetFileInfo("index.html");
var serveSpa = spaIndex.Exists;
if (serveSpa)
{
    app.UseStaticFiles(); // Serve the React files from wwwroot
}
else
{
    app.Logger.LogInformation(
        "No frontend build found in wwwroot. Static file hosting is disabled; build the UI to wwwroot or set FrontendURL to your hosted UI.");
}
app.UseRateLimiter();

// CORS must run before Auth
app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (serveSpa)
{
    app.MapFallbackToFile("index.html"); // Handle client-side routing
}

// Seed database with roles and admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var dbContext = services.GetRequiredService<AppDbContext>();

        // Create Admin role if it doesn't exist
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
            logger.LogInformation("'Admin' role created.");
        }

        // Create a default admin user if it doesn't exist
        var adminEmail = "admin@ecommerce.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new ApplicationUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            var adminPassword = builder.Configuration["AdminUser:Password"] ?? throw new InvalidOperationException("Admin password not configured");
            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                logger.LogInformation("Default admin user created and assigned to 'Admin' role.");
            }
        }

        // Seed database with sample product and sales data
        await DataSeeder.SeedAsync(dbContext, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during database seeding.");
    }
}

app.Run();

public partial class Program { }
