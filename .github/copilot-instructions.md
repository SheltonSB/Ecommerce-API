## Purpose
Short, actionable guidance for AI coding agents working on the Ecommerce-API solution.

## Quick start (commands)
- Build the solution:

```
dotnet build "Ecommerce.sln"
```

- Run the API in Development (uses SQLite, Swagger at root):

```
dotnet run --project Ecommerce.Api
```

- Run tests:

```
dotnet test "Ecommerce.sln"
```

Notes: the app will call `context.Database.MigrateAsync()` and `Seed.Initialize(context)` on startup (see `Program.cs`). For manual EF migrations you can use the dotnet-ef tools if available, but migrations are applied automatically at runtime.

## Big-picture architecture
- ASP.NET Core Web API (root project: `Ecommerce.Api`).
- Data layer: `Ecommerce.Api/Data/AppDbContext.cs` using EF Core. Entities live under `Ecommerce.Api/Domain`.
- Contracts/DTOs: `Ecommerce.Api/Contracts` (validation via DataAnnotations).
- Business logic: `Ecommerce.Api/Services/*` (services are registered as Scoped in `Program.cs`). Controllers are thin (map endpoints to services).
- Cross-cutting: `Ecommerce.Api/Infrastructure` contains middleware and query helpers used across services.
- Logging: Serilog configured in `Program.cs` and `appsettings*.json`, writes to `logs/ecommerce-api-.txt`.

## Important files to inspect or change
- `Program.cs` — DI registrations, environment behavior (SQLite in Development, SQL Server otherwise), Serilog, Swagger, automatic migrations/seeding.
- `Data/AppDbContext.cs` — entity configuration, query filters, seeding logic in `SeedData`.
- `Infrastructure/QueryableExtensions.cs` — central helpers: Paginate, SortBy, WhereIf, SearchIn. Services rely on these for filtering/paging/sorting.
- `Infrastructure/GlobalExceptionMiddleware.cs` — converts common exceptions (ArgumentException, InvalidOperationException, KeyNotFoundException, UnauthorizedAccessException, TimeoutException) into standard HTTP responses. Services intentionally throw these exception types to signal 4xx/404 conditions.
- `Services/*` — business logic lives here (e.g., `ProductService.cs` shows typical patterns and how queries are built).
- `Contracts/*.cs` — DTOs and validation attributes; use these to understand request/response shapes.

## Project-specific patterns and conventions
- Soft-delete + query filters: Entities inherit `Entity` and use an `IsDeleted` flag. `AppDbContext` applies `HasQueryFilter(e => !e.IsDeleted)` for most entities — use `IgnoreQueryFilters()` when you need deleted rows (see `RestoreAsync`).
- Timestamp handling: `SaveChanges()` / `SaveChangesAsync()` call `UpdateTimestamps()` which sets `CreatedAt` and `UpdatedAt`. Prefer entity methods (e.g., `UpdateTimestamp()`, `MarkAsDeleted()`) where provided.
- Exception-driven flow: Services throw framework exceptions (KeyNotFoundException -> 404, InvalidOperationException -> 400). Do not swallow these in controllers; let the global middleware map them to HTTP responses.
- Query composition: Services build EF queries using `QueryableExtensions` helpers. Example (from `ProductService`):

```
query = query.WhereIf(categoryId.HasValue, p => p.CategoryId == categoryId.Value)
             .SearchIn(searchTerm, p => p.Name, p => p.Description, p => p.Sku)
             .SortBy(request.SortBy, request.SortDirection)
             .Paginate(request.Page, request.PageSize);
```

- DTO validation: Use DataAnnotations on Contracts; controllers will rely on model validation.

## Data & seeding
- `AppDbContext.OnModelCreating` seeds initial categories and demo products. On startup the app applies migrations and then calls `Seed.Initialize(context)` (see `Data/Seed.cs`). When modifying models, add migrations and ensure the seeding logic remains consistent.

## Tests and expected test patterns
- Tests live under `Ecommerce.Tests`. The repo uses standard xUnit/NUnit (check the project file) and `dotnet test` runs them.
- Services are unit-test targets; prefer testing service behavior by mocking `AppDbContext` or using an in-memory database. Tests should assert thrown exceptions for invalid operations (e.g., creating a product with duplicate SKU) and happy-path DTOs returned by service methods.

## Runtime and environment notes
- Development uses SQLite (connection string in `appsettings.Development.json`). Production expects SQL Server.
- Swagger UI is enabled in Development and is hosted at the app root (Program.cs sets RoutePrefix = string.Empty).

## Logging & diagnostics
- Serilog is configured and used across the app. Logs are written to `logs/ecommerce-api-.txt` with daily rolling.
- Health check endpoint: `GET /health`.

## When editing code, common places to update together
- If you add or change an EF entity: update `Domain/*`, update `AppDbContext` model configuration, add migration (or rely on runtime migration), and adjust `Data/Seed.cs` if initial data must change.
- If you change DTOs in `Contracts/*`, update service return types and controller signatures.

## Minimal examples to reference
- Product list flow: `Services/ProductService.cs` uses `QueryableExtensions` and returns `Contracts/Paged<T>` results. Use that as the canonical pattern for paging/filtering/sorting.
- Error handling: throw `KeyNotFoundException` when resource not found; `GlobalExceptionMiddleware` maps it to 404.

If anything here is unclear or you want the file to include more examples (e.g., test patterns, exact EF migration steps), tell me which area to expand and I will iterate.
