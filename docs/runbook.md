# LuxeStore Runbook (Dev/Staging)

## Environment
- API env vars:
  - `ConnectionStrings__DefaultConnection`
  - `Jwt__Key`, `Jwt__Issuer`, `Jwt__Audience`
  - `Stripe__PublishableKey`, `Stripe__SecretKey`
  - `Cloudinary__CloudName`, `Cloudinary__ApiKey`, `Cloudinary__ApiSecret`
  - `EmailSettings__Host`/`Port`/`From`/`UserName`/`Password`/`EnableSsl`
  - `AdminSeed__Email`/`Password` (optional; seed admin)
- Frontend:
  - `VITE_API_BASE_URL`
  - Set `Redis:ConnectionString` in prod to enable distributed cache.

## Commands
- API dev: `dotnet run --launch-profile https`
- API migrate: `dotnet ef database update --connection "<conn>"`
- API tests: `dotnet test`
- Frontend dev: `npm run dev` (from `frontend/`)
- Frontend build: `npm run build`
- Frontend tests: `npm run test` (coverage thresholds enforced), `npm run coverage`

## Admin Seed
- Set `AdminSeed__Email` and `AdminSeed__Password`; seeder runs at startup. Change password after first login. Omit these to skip seeding.

## Deployment Notes
- Don’t store secrets in git; use env vars.
- Ensure CORS allows your frontend origin.
- Health: `/health`; Rate limiting enabled (global + authLimiter).
- Metrics: `/metrics` (Prometheus scraping).
- Logs: Serilog console/file (add a sink in prod). To emit traces, configure OTLP exporter via env (e.g., `OTEL_EXPORTER_OTLP_ENDPOINT`).
- Alerts: monitor 5xx rate, p95 latency for auth/products/uploads, cache hits/misses, rate-limit rejections.

## Known Warnings
- Stripe package resolves to latest minor; pin if needed.
- Vitest tests run in watch mode locally; use `q`/Ctrl+C to exit.
