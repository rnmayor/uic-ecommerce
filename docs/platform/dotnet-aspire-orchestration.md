# .NET Aspire — local orchestration

**Status:** Optional — implement when modular monolith migration is stable.
**Audience:** Developers running the backend locally with multiple resources (API, database, future workers).
**Prerequisite:** [Migration playbook](../migration-playbook-from-layered-monolith-to-modular-monolith.md) module work substantially complete; host is `Ecommerce.Api` with `ModulesRegistry` wiring bounded contexts.

---

## What this gives you

| Without Aspire | With Aspire |
|----------------|-------------|
| Manual `dotnet run`, Docker Compose, or Railway-only workflow | **AppHost** starts API + Postgres (and later workers) together |
| Connection strings in local env / user secrets | **Service discovery** and injected connection strings in dev |
| Logs in terminal only | **Aspire dashboard** — resources, logs, traces (when OTel is wired) |

Aspire does **not** replace Railway for production. It improves **local and team dev** when the system has more moving parts (outbox worker, search container, message broker emulator).

---

## When to adopt

**Good time:**

- You run an **outbox processor** or **Notifications** worker as a separate process.
- You want one command (`dotnet run` on AppHost) for new contributors.
- You add **Redis**, **RabbitMQ**, or **Meilisearch** locally.

**Defer if:**

- Single API + one Postgres and `dotnet run` is enough.
- You are still mid-migration and want minimal project churn.

---

## Target layout (after migration)

Assumes legacy `Ecommerce.Application` / `Infrastructure` are retired or thin; modules live under `src/Modules/`.

```text
backend/ecommerce-api/
├── Ecommerce.AppHost/              ← new (Aspire host)
├── Ecommerce.ServiceDefaults/      ← new (shared OTel/health/resilience — optional but recommended)
├── src/
│   ├── Ecommerce.Api/              ← existing API (reference ServiceDefaults)
│   ├── Modules/...
│   └── Shared/...
└── Ecommerce.slnx                  ← add AppHost + ServiceDefaults
```

Railway continues to deploy **`Ecommerce.Api`** (or your Docker image) — **not** the AppHost.

---

## Setup instructions (apply when ready)

### Step 1 — Install tooling

```powershell
dotnet workload update
dotnet workload install aspire
```

Verify:

```powershell
dotnet aspire --version
```

### Step 2 — Add Aspire projects (from `backend/ecommerce-api/`)

```powershell
cd backend/ecommerce-api

dotnet new aspire-apphost -n Ecommerce.AppHost -o Ecommerce.AppHost
dotnet new aspire-servicedefaults -n Ecommerce.ServiceDefaults -o Ecommerce.ServiceDefaults
```

Add both projects to `Ecommerce.slnx`.

### Step 3 — Reference ServiceDefaults from the API

In `src/Ecommerce.Api/Ecommerce.Api.csproj`:

```xml
<ProjectReference Include="..\..\Ecommerce.ServiceDefaults\Ecommerce.ServiceDefaults.csproj" />
```

In `Program.cs`, at the top of service registration (after `WebApplication.CreateBuilder`):

```csharp
builder.AddServiceDefaults();
```

Before `app.Run()`, after pipeline configuration:

```csharp
app.MapDefaultEndpoints(); // health checks for orchestrator
```

*(Exact extension names come from the generated `ServiceDefaults` template — match your generated file.)*

### Step 4 — Wire AppHost to API + PostgreSQL

In `Ecommerce.AppHost/Program.cs` (conceptual — adjust to generated template):

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .AddDatabase("ecommerce");

builder.AddProject<Projects.Ecommerce_Api>("ecommerce-api")
    .WithReference(postgres)
    .WithExternalHttpEndpoints();

builder.Build().Run();
```

**Connection string:** Aspire injects references into the API as configuration (e.g. `ConnectionStrings__ecommerce` or your `DatabaseOptions` binding). Update `DatabaseOptions` binding or add a small dev-only configuration adapter so `TenancyDbContext` and other module contexts receive the same connection string.

If modules register their own `DbContext` in `*Module` extensions, they should all read the **same** connection string key in dev and production.

### Step 5 — Run locally

```powershell
dotnet run --project Ecommerce.AppHost
```

Open the Aspire dashboard URL printed in the console (typically `https://localhost:15xxx`).

### Step 6 — Optional: add a worker later

When you implement an outbox or integration-event worker:

```csharp
builder.AddProject<Projects.Ecommerce_OutboxWorker>("outbox-worker")
    .WithReference(postgres);
```

Same Postgres database; worker reads outbox table — aligns with [Event-driven maturity ladder](../migration-playbook-from-layered-monolith-to-modular-monolith.md#9-event-driven-maturity-ladder).

---

## Railway / production

| Concern | Approach |
|---------|----------|
| Deploy target | Keep deploying **`Ecommerce.Api`** container to Railway |
| AppHost | **Dev only** — do not deploy AppHost to Railway |
| Postgres | Railway plugin or external Postgres; connection string in Railway env vars |
| Module DbContexts | Same connection string as today; Aspire only changes **how** local dev injects it |

---

## Verification checklist

- [ ] `dotnet run --project Ecommerce.AppHost` starts API and Postgres
- [ ] Health endpoint responds (`/health` or Aspire defaults)
- [ ] Tenancy onboarding still works against Aspire Postgres
- [ ] Railway deploy unchanged (no AppHost in Dockerfile)

---

## Troubleshooting

| Issue | Check |
|-------|--------|
| API cannot connect to DB | Connection string key matches `DatabaseOptions` / module registration |
| Port conflict | AppHost assigns ports; API `PORT` env may differ from Railway — local only |
| Multiple DbContexts | All module contexts use same database URL in dev |

---

## References

- [.NET Aspire documentation](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [Migration playbook 14](../migration-playbook-from-layered-monolith-to-modular-monolith.md#14-platform-extras-optional--not-migration-phases)
- [OpenTelemetry observability](opentelemetry-observability.md) — pair with ServiceDefaults for dashboard traces

---

*Apply this guide after module migration. Update paths if your solution structure differs from the target layout above.*
