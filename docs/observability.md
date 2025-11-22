# Observability Quick Notes

- Metrics: OpenTelemetry metrics enabled with AspNetCore + EF instrumentation. Prometheus scraping endpoint at `/metrics`.
- Tracing: Resource name `Ecommerce.Api` set; to emit traces, add OTLP/Jaeger exporter package and set `OTEL_EXPORTER_OTLP_ENDPOINT` (and related envs). Correlate with `traceId` from error responses.
- Errors: Global exception middleware returns structured `{ statusCode, message, details?, timestamp, traceId }`. TraceId can be used to correlate logs/traces.
- Suggested alerts: 5xx rate, latency p95/p99 for key endpoints (auth, products, admin uploads), cache misses, rate-limit rejections.
- Logging: Serilog console/file; for production, add a sink (Seq/ELK) and correlate with `traceId`.
