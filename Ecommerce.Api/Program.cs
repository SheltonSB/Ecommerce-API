using AutoMapper;
using Ecommerce.Api.Data;
using Ecommerce.Api.Domain;
using Ecommerce.Api.Infrastructure;
using Ecommerce.Api.Services;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// 1. Database (Neon Postgres)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("EcommerceTests"));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));
}

// 2. Auth (Identity)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// 3. JWT Configuration
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new Exception("JWT Key missing");
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
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
    options.AddPolicy("AllowReactApp", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader());
});

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<ImageService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddMemoryCache();

var redisConnection = builder.Configuration["Redis:ConnectionString"];
if (string.IsNullOrWhiteSpace(redisConnection))
{
    redisConnection = builder.Configuration.GetConnectionString("Redis");
}

if (!string.IsNullOrWhiteSpace(redisConnection))
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConnection;
        options.InstanceName = "Ecommerce_";
    });
    builder.Services.AddScoped<ICacheProvider, DistributedCacheProvider>();
}
else
{
    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddScoped<ICacheProvider, MemoryCacheProvider>();
}

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Ecommerce.Api"))
        .AddAspNetCoreInstrumentation()
        .AddPrometheusExporter())
    .WithTracing(tracing => tracing
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Ecommerce.Api"))
        .AddAspNetCoreInstrumentation()
        .AddEntityFrameworkCoreInstrumentation()
        .AddOtlpExporter());
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("authLimiter", context =>
        RateLimitPartition.GetFixedWindowLimiter(context.Connection.RemoteIpAddress?.ToString() ?? "anon", _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 20,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
        }));

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var key = context.Connection.RemoteIpAddress?.ToString() ?? "anon";
        return RateLimitPartition.GetFixedWindowLimiter(key, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 100,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
        });
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var key = context.Connection.RemoteIpAddress?.ToString() ?? "anon";
        return RateLimitPartition.GetFixedWindowLimiter(key, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 100,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
        });
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// Register Services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ISaleService, SaleService>();

var app = builder.Build();

// Configure Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowReactApp");
}
else
{
    // Support array or comma-separated env var (e.g., Cors__ProductionOrigins="https://a.com,https://b.com")
    var prodOrigins = builder.Configuration.GetSection("Cors:ProductionOrigins").Get<string[]>() ?? Array.Empty<string>();
    if (prodOrigins.Length == 0)
    {
        var csv = builder.Configuration["Cors:ProductionOrigins"];
        prodOrigins = string.IsNullOrWhiteSpace(csv)
            ? new[] { "https://your-production-domain.com" }
            : csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    app.UseHsts();
    app.UseCors(policy =>
        policy.WithOrigins(prodOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader());
}

app.UseHttpsRedirection();
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.UseMiddleware<AuditMiddleware>();

// Seed admin role/user if configured
var adminEmail = builder.Configuration["AdminSeed:Email"];
var adminPassword = builder.Configuration["AdminSeed:Password"];
if (!string.IsNullOrWhiteSpace(adminEmail) && !string.IsNullOrWhiteSpace(adminPassword))
{
    await AdminSeeder.SeedAsync(app.Services, adminEmail, adminPassword);
}

app.Run();

// Expose Program to integration tests
public partial class Program { }
