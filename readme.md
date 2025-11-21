

-----

# LuxeStore Commerce Platform

Modern e-commerce API + React frontend built with production practices: Identity + JWT auth with email verification, Neon Postgres, Stripe-ready plumbing, analytics tracking, and a minimalist Luxe UI.

## Highlights
- ASP.NET Core 9 API with Identity (email confirmation), JWT auth, Serilog logging, EF Core (Npgsql), Stripe-ready services, and analytics (UserInteractions) for AI-ready data.
- React + Vite + TypeScript + Tailwind frontend (LuxeStore design system) with Inter font, axios interceptors, hot toast notifications, and a curated home/products experience.
- Migrations and seeding via EF Core; automated Swagger/OpenAPI; health checks; CORS policy for frontend origin.

## Architecture
- Domain: Entities (Products, Categories, Sales, PaymentInfo, PriceHistory, UserInteraction), ApplicationUser (Identity).
- Data: EF Core (Npgsql) with migrations; design-time factory for tooling; soft deletes and audit fields.
- Services: Products/Categories/Sales, EmailSender (SMTP-configurable), analytics tracker endpoints.
- API: Controllers for auth (register/login/verify email), products, categories, sales, analytics; global exception middleware.
- Frontend: Vite React TS, Tailwind, Lucide icons; pages for Home, Products, Login, Register, Verify Email; shared UI components.

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
- `Frontend:BaseUrl` (used to build verify-email links; defaults to request host)

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

## Frontend Pages
- `/home`: Luxe hero + highlights
- `/products`: public catalog from API
- `/register`: first/last/phone/email/password, prompts to verify email
- `/login`: redirects to verify if email unconfirmed
- `/verify-email`: handles token/userId from email link

## Testing
```bash
dotnet test
# Frontend lint not configured; add ESLint/Prettier as desired
```

## Security Notes
- Do not commit real secrets (DB, Stripe, JWT, SMTP). Use env vars in production.
- CORS: allow your frontend origin (default http://localhost:5173 for dev).
- HTTPS enforced in production; dev profile supports http/https.

## Deployment Tips
- API: Render Web Service with `dotnet publish -c Release -o out` and `dotnet Ecommerce.Api.dll`; set env vars and CORS origin.
- DB: Neon Postgres with TLS (`Ssl Mode=Require;Trust Server Certificate=true` or as required).
- Frontend: Vercel; set `VITE_API_BASE_URL` to your deployed API URL.

## Rapid Verify Flow (no SMTP)
- Register via `/api/auth/register` or the UI.
- Capture `token` + `userId` from logs (or temporarily log them) and POST to `/api/auth/verify-email`, or open `/verify-email` page and paste values.

## Build/Run Commands (Quick Reference)
- Restore/build API: `dotnet restore && dotnet build`
- Run API dev: `dotnet run --launch-profile https`
- Frontend dev: `npm run dev` (in `frontend`)
- Frontend build: `npm run build`
