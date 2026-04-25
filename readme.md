# Northstar Commerce

Northstar Commerce is now a single-host .NET 9 application with:

- A customer storefront at `/`
- An admin console at `/admin`
- Swagger/OpenAPI docs at `/docs`
- REST endpoints under `/api/*`
- A health endpoint at `/health`

The API, client, and admin UI are deployed together from the same ASP.NET Core project, so there is no missing frontend build step anymore.

## Stack

- ASP.NET Core 9
- Entity Framework Core 9
- SQLite by default, SQL Server when `DatabaseProvider=SqlServer`
- Serilog
- xUnit, FluentAssertions, Moq

## Local Run

```bash
dotnet restore
dotnet run --project Ecommerce.Api
```

Open:

- `http://localhost:5154/` for the storefront
- `http://localhost:5154/admin` for the admin console
- `http://localhost:5154/docs` for Swagger

The app migrates the database on startup and seeds categories, products, sales, and price history idempotently.

## Test And Publish

```bash
dotnet build Ecommerce.sln
dotnet test Ecommerce.sln
dotnet publish Ecommerce.Api/Ecommerce.Api.csproj -c Release
```

## Docker

Build:

```bash
docker build -t northstar-commerce .
```

Run:

```bash
docker run --rm -p 8080:8080 northstar-commerce
```

The container defaults to SQLite with the database stored at `/app/data/Ecommerce.db`.

## Configuration

Defaults live in [Ecommerce.Api/appsettings.json](/C:/Users/shelt/OneDrive/Desktop/Ecommerce-API/Ecommerce.Api/appsettings.json:1).

Useful overrides:

- `DatabaseProvider=Sqlite`
- `DatabaseProvider=SqlServer`
- `ConnectionStrings__DefaultConnection=<your connection string>`

## Notes

- Product updates now support price changes and write price-history records.
- The storefront checkout creates a sale, records payment, and completes the order through the API.
- The admin console can create and update categories/products, inspect orders, add payment, and complete or cancel pending sales.
