

-----

# LuxeStore Commerce Platform

Modern e-commerce API built with production practices: Identity + JWT auth with email verification, Postgres, Stripe-ready plumbing, analytics tracking (now with linear-regression forecasting), and a minimalist Luxe UI (frontend removed in this deployment path).

## Highlights
- ASP.NET Core 9 API with Identity (email confirmation), JWT auth, Serilog logging, EF Core (Npgsql), Stripe-ready services, and analytics (UserInteractions) for AI-ready data.
- React + Vite + TypeScript + Tailwind frontend (LuxeStore design system) with Inter font, axios interceptors, hot toast notifications, and a curated home/products experience.
- Migrations and seeding via EF Core; automated Swagger/OpenAPI; health checks; CORS policy for frontend origin.

## How to Run
- Backend: `cd Ecommerce.Api && dotnet run` (uses CORS policy `AllowReactApp` for `http://localhost:5173` by default).
- Frontend: removed for current deployment path. Point `Frontend:BaseUrl` to your hosted UI if you add one later.
- Seeding: The API seeds sample data at startup via `DataSeeder.SeedAsync`; you can also POST `/api/DataSeeding/seed` from Swagger to repopulate quickly.
- Test account: Admin user is created at startup as `admin@ecommerce.com` with password from `AdminUser:Password` in `appsettings.json` (set a strong value before running).

## Architecture
- Domain: Entities (Products, Categories, Sales, PaymentInfo, PriceHistory, UserInteraction), ApplicationUser (Identity).
- Data: EF Core (Npgsql) with migrations; design-time factory for tooling; soft deletes and audit fields.
- Services: Products/Categories/Sales, EmailSender (SMTP-configurable), analytics tracker endpoints.
- API: Controllers for auth (register/login/verify email), products, categories, sales, analytics; global exception middleware.
- Frontend: previously Vite/React; removed in this deployment. API remains fully functional.

## Prerequisites
- .NET 9 SDK
- Node 18+ (for frontend)
- Postgres connection (Neon recommended)
- SMTP creds (for real email) or leave EmailSender in no-op mode for local dev

## Configuration (API)
Set via `appsettings.json` or environment variables:
- `ConnectionStrings:DefaultConnection` (Neon/Postgres)
- `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience`
- `Stripe:PublishableKey`, `Stripe:SecretKey` (placeholders OK until live)
- `EmailSettings:Host`, `Port`, `From`, `UserName`, `Password`, `EnableSsl` (optional; if missing, registration still works and email send is skipped with a log warning)
- `Frontend:BaseUrl` (used to build verify-email links; defaults to request host). Set to your real UI domain if present.

## Run Locally
API:
```bash
cd Ecommerce.Api
dotnet run --launch-profile https   # listens on https://localhost:7154 and http://localhost:5154
```

Frontend:
```bash
cd frontend
echo "VITE_API_BASE_URL=https://localhost:7154/api" > .env
npm install
npm run dev   # http://localhost:5173
```

## Database & Migrations
Create/update schema on Neon:
```bash
cd Ecommerce.Api
dotnet ef database update
```
To add a migration:
```bash
dotnet ef migrations add <Name>
dotnet ef database update
```

## Key Endpoints (public where noted)
- Auth: `POST /api/auth/register` (sends verify link), `POST /api/auth/login` (blocks unconfirmed email), `POST /api/auth/verify-email`
- Products (public): `GET /api/products`, `GET /api/products/{id}`, `GET /api/products/sku/{sku}`
- Categories: `GET /api/categories`, `POST /api/categories`, `PUT /api/categories/{id}`
- Sales: `GET /api/sales`, `POST /api/sales`, `POST /api/sales/{id}/complete`
- Analytics: `POST /api/analytics/track`, `GET /api/analytics/export`
- Health: `GET /health`

## Frontend
- Removed from this repo for this deployment path. If you add one, set `VITE_API_BASE_URL` to the deployed API URL and `Frontend:BaseUrl` accordingly.

## Testing
```bash
dotnet test
<<<<<<< HEAD
=======
```

## Security Notes
- Do not commit real secrets (DB, Stripe, JWT, SMTP). Use env vars in production.
- CORS: allow your frontend origin (default http://localhost:5173 for dev).
- HTTPS enforced in production; dev profile supports http/https.

## Deployment Tips
- API: containerize (Dockerfile provided) and run via App Service/Container Apps; set env vars and CORS origin.
- DB: Postgres with TLS (`Ssl Mode=Require;Trust Server Certificate=true` or as required).
- Frontend: add separately if needed; set `VITE_API_BASE_URL` to your deployed API URL.

## Rapid Verify Flow (no SMTP)
- Register via `/api/auth/register` or the UI.
- Capture `token` + `userId` from logs (or temporarily log them) and POST to `/api/auth/verify-email`, or open `/verify-email` page and paste values.

## Build/Run Commands (Quick Reference)
- Restore/build API: `dotnet restore && dotnet build`
- Run API dev: `dotnet run --launch-profile https`
- Frontend commands removed (frontend directory deleted in this deployment path).

## Docker
- `docker-compose.yml` now runs API + Postgres only. `docker compose up --build` brings both up; migrations run automatically on startup.
>>>>>>> bfdd4a1 (implemented checkout and photos)
