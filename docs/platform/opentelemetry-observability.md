# OpenTelemetry observability (with Serilog)

**Status:** Optional — implement when modular monolith migration is stable and you want traces/metrics beyond stdout logs.
**Audience:** Developers operating the API on Railway today; optionally exporting to Azure Monitor or another OTLP backend later.
**Prerequisite:** Working `Ecommerce.Api` host with Serilog (see `appsettings.json` → `Serilog` section).

---

## Concepts (read this first)

| Technology | Role in this repo |
|------------|-------------------|
| **Serilog** | **Logging** — structured logs to console (Railway captures stdout). Already configured in `Program.cs` via `UseSerilog`. |
| **OpenTelemetry (OTel)** | **Traces** (request/handler spans) and **metrics** (rates, durations). Vendor-neutral; you pick an **exporter**. |
| **Application Insights** | **Azure’s APM product** — one possible **backend** for OTel data via `Azure.Monitor.OpenTelemetry.AspNetCore`. Not required for OTel. |
| **Railway logs** | Log **viewer** for container stdout — not a trace UI. OTel traces need an exporter (Aspire dashboard locally, Azure, Grafana Cloud, Jaeger, etc.). |

**Serilog is not replaced by OTel.** Common pattern:

- **Logs:** Serilog → Console (Railway) → optionally also OTel log bridge or App Insights sink.
- **Traces/metrics:** OTel SDK → exporter of your choice.

You do **not** need Azure hosting to use Application Insights ingestion (connection string from Azure portal works from Railway).

---

## When to adopt

**Good time:**

- Migration complete; you debug **cross-module** flows (HTTP → middleware → dispatcher → handler → event).
- You add **outbox workers** or background consumers and need correlated traces.
- You adopt **.NET Aspire** locally — ServiceDefaults includes OTel wiring for the dashboard.

**Defer if:**

- Structured console logs on Railway are sufficient.
- You are still moving bounded contexts — avoid observability churn during migration.

---

## Recommended phased rollout

| Phase | What you add | Railway impact |
|-------|--------------|------------------|
| **A** | OTel traces + metrics → **Console** exporter (dev only) | None if dev-only |
| **B** | ASP.NET Core + `HttpClient` instrumentation | Minimal overhead |
| **C** | Custom spans around `ICommandDispatcher` / `IEventPublisher` (optional) | Better module visibility |
| **D** | Exporter: **Aspire dashboard** (local) or **Azure Monitor** / **Grafana Cloud OTLP** | Env vars for connection string / OTLP endpoint |
| **E** | Optional: Serilog → Application Insights sink **or** OTel log bridge | Dual path: stdout + cloud |

Start with **A + B** locally; add **D** when you pick a backend.

---

## Setup instructions (apply when ready)

Assumes post-migration host: `backend/ecommerce-api/src/Ecommerce.Api/`.

### Step 1 — Packages (API project)

```powershell
cd backend/ecommerce-api/src/Ecommerce.Api

dotnet add package OpenTelemetry.Extensions.Hosting
dotnet add package OpenTelemetry.Instrumentation.AspNetCore
dotnet add package OpenTelemetry.Instrumentation.Http
dotnet add package OpenTelemetry.Instrumentation.Runtime
dotnet add package OpenTelemetry.Exporter.Console
```

**If using .NET Aspire ServiceDefaults:** many packages are already referenced — skip duplicates and use `builder.AddServiceDefaults()` from [Aspire guide](dotnet-aspire-orchestration.md).

**Optional — Azure Monitor exporter (only if you use Application Insights):**

```powershell
dotnet add package Azure.Monitor.OpenTelemetry.AspNetCore
```

**Optional — OTLP exporter (Grafana Cloud, Jaeger, etc.):**

```powershell
dotnet add package OpenTelemetry.Exporter.OpenTelemetryProtocol
```

### Step 2 — Register OpenTelemetry in `Program.cs`

Add **after** `WebApplication.CreateBuilder(args)` and **alongside** existing Serilog setup (keep `UseSerilog`):

```csharp
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

// ... inside Program.cs, after var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(serviceName: "Ecommerce.Api", serviceVersion: "1.0.0"))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddConsoleExporter()) // Phase A: dev only — remove or gate in production
    .WithMetrics(metrics => metrics
        .AddRuntimeInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddConsoleExporter());
```

**Gate console exporter in production** — use environment check:

```csharp
if (builder.Environment.IsDevelopment())
{
    tracing.AddConsoleExporter();
}
```

### Step 3 — Keep Serilog as-is for Railway

Do **not** remove:

```csharp
builder.Host.UseSerilog((context, services, configuration) => { ... });
```

Railway continues to show Serilog structured output in the log tab. OTel traces go to the exporter you configure — stdout console exporter is for local debugging only.

Ensure existing enrichers remain useful for correlation:

- `LogEnrichmentMiddleware` — TraceId, UserId, TenantId (when migration complete)
- When OTel is enabled, consider adding **TraceId** from `Activity.Current` into Serilog log context so logs and traces join in backends that support it.

### Step 4 — Optional: Application Insights (Azure backend)

1. Create Application Insights resource in Azure portal.
2. Copy **Connection String**.
3. On Railway (or user secrets locally), set:

```text
APPLICATIONINSIGHTS_CONNECTION_STRING=InstrumentationKey=...;IngestionEndpoint=...
```

4. Replace manual `AddOpenTelemetry` with:

```csharp
builder.Services.AddOpenTelemetry()
    .UseAzureMonitor(); // Azure.Monitor.OpenTelemetry.AspNetCore
```

Or combine Azure Monitor with custom resource attributes. Traces and metrics appear in Azure portal; logs can stay on Serilog stdout unless you add AI log exporter.

**You do not need to host the API on Azure App Service** — Railway + AI connection string is valid.

### Step 5 — Optional: OTLP exporter (non-Azure)

Set environment variables (example for Grafana Cloud / generic OTLP):

```text
OTEL_EXPORTER_OTLP_ENDPOINT=https://your-collector:4317
OTEL_EXPORTER_OTLP_HEADERS=Authorization=Basic ...
```

Register:

```csharp
tracing.AddOtlpExporter();
metrics.AddOtlpExporter();
```

### Step 6 — Optional: spans for modular monolith (Phase C)

After migration, add thin instrumentation without changing business logic:

- **Middleware:** ASP.NET instrumentation already creates a server span.
- **Dispatchers:** wrap `ICommandDispatcher.DispatchAsync` / `IQueryDispatcher.DispatchAsync` with `ActivitySource.StartActivity("Command {Name}")`.
- **Events:** wrap `IEventPublisher.PublishAsync` with a span per event type.

Use a shared `ActivitySource` name e.g. `Ecommerce.Shared` in `Ecommerce.Shared.Infrastructure`.

This makes module boundaries visible in trace UIs without coupling modules to a specific vendor.

### Step 7 — Verify locally

1. Run API: `dotnet run --project src/Ecommerce.Api`
2. Call health or a tenancy endpoint.
3. With **Console exporter**, see span output in the terminal.
4. With **Aspire AppHost**, open dashboard **Traces** tab.
5. With **Application Insights**, wait 1–2 minutes and check **Transaction search** in Azure portal.

---

## Railway deployment notes

| Signal | Railway today | With OTel |
|--------|---------------|-----------|
| **Logs** | Serilog → stdout → Railway log viewer | Same — keep this path |
| **Traces** | Not available in Railway UI | Export to Azure / Grafana / Honeycomb via env vars |
| **Metrics** | Not available in Railway UI | Same as traces — needs exporter |
| **Cost** | Included in Railway | Azure/Grafana may have free tiers; watch ingestion volume |

**Recommendation for Railway-only phase:** Serilog + good structured properties (`TenantId`, `UserId`, `TraceId`) until you choose a trace backend.

---

## Serilog vs OTel — decision summary

| Question | Answer |
|----------|--------|
| Is OTel a Serilog sink? | No — different APIs; can integrate via bridges or dual export |
| Must I use Azure? | No — OTLP works with many vendors |
| Replace Serilog on Railway? | **No** — keep stdout logging for platform logs |
| Same as Application Insights? | AI is a **backend**; OTel is how you **produce** telemetry |

---

## Verification checklist

- [ ] Serilog still writes to console; Railway logs unchanged
- [ ] Traces appear in chosen backend (or console in dev)
- [ ] HTTP requests create spans with status codes
- [ ] No duplicate registration if using Aspire ServiceDefaults
- [ ] Production does not use Console exporter unless intentional

---

## References

- [OpenTelemetry .NET](https://opentelemetry.io/docs/languages/net/)
- [Azure Monitor OpenTelemetry for ASP.NET Core](https://learn.microsoft.com/en-us/azure/azure-monitor/app/opentelemetry-enable?tabs=aspnetcore)
- [.NET Aspire orchestration](dotnet-aspire-orchestration.md) — ServiceDefaults + dashboard
- [Migration playbook 14](../migration-playbook-from-layered-monolith-to-modular-monolith.md#14-platform-extras-optional--not-migration-phases)
- [Event-driven maturity ladder](../migration-playbook-from-layered-monolith-to-modular-monolith.md#9-event-driven-maturity-ladder) — outbox/workers benefit most from traces

---

*Apply this guide after module migration. Adjust package versions to match your target `net10.0` / SDK when you implement.*
