# LuxeStore Technical Decisions (Snapshot)

- **Stack:** React + Vite + TS + Tailwind with Radix primitives and cva for headless, composable UI. ASP.NET Core + EF Core + Identity + JWT on the backend for mature tooling and auth.
- **Styling:** HSL tokens via CSS variables for easy theming/dark mode; Cormorant for display type, Inter for body; tailwindcss-animate for motion.
- **Data & Auth:** Postgres (Neon) + EF Core migrations. Email verification enforced; JWT for clients; plan to gate admin routes by roles.
- **Media:** Cloudinary for image hosting; store only URLs in DB.
- **Caching:** In-memory cache for product listings; Redis planned for multi-instance.
- **Frontend data:** Axios with interceptors; mock data fallback when API unavailable.
- **Build/CI:** GitHub Actions to restore/build/test API and build frontend. Env samples provided to avoid committing secrets.
- **Security:** Secrets via env vars (no real keys in git). CORS per environment. Admin endpoints to be locked with roles + rate limits.
