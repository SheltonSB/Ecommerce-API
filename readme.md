# LuxeStore Commerce Platform

ASP.NET Core 9 e-commerce API with a React + Vite + TypeScript frontend in the same `Ecommerce.Api` folder.

## Run Locally

1. API
```bash
cd Ecommerce.Api
dotnet run --launch-profile https
```

2. Frontend
```bash
cd Ecommerce.Api
npm install
echo "VITE_API_URL=https://localhost:7154" > .env
npm run dev
```

Frontend default URL is `http://localhost:5173`.

## Required Configuration

Set these via `appsettings.*.json` or environment variables:

- `ConnectionStrings:DefaultConnection`
- `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience`
- `Stripe:SecretKey`, `Stripe:PublishableKey`
- `AdminUser:Password`
- `FrontendURL` (production frontend origin)

Render env examples are in `render.yaml`.

## Notes

- CORS allows configured frontend origins and localhost defaults in development.
- Checkout uses Stripe Checkout Sessions and client-side Stripe.js redirect.
- Admin endpoints require `Admin` role claims in JWT.

## Build Checks

```bash
cd Ecommerce.Api
dotnet build
npm run build
```
