# Migration Playbook: From Layered Monolith to Modular Monolith (Event-Ready)

**Status:** Living document — evolves as this repository migrates.
**Audience:** Mid-level to senior engineers and architects comfortable with DDD language, multi-tenancy, and incremental refactors. Junior developers may need a companion glossary for terms like bounded context, outbox, and integration event.
**Scope:** This repository’s path from a **single deployable API** with classic layers plus **vertical slices** (ADR-0003) toward a **modular monolith** with bounded-context modules (ADR-0005), then toward **event-driven** collaboration **without** requiring microservices on day one.

---

## How to use this document

1. Read **Architectural context** and **Where we started** to align mental models with this codebase.
2. Read **Target architecture** and **Strategic decisions** before changing project structure.
3. Use **Per-module phase template** as a repeatable checklist for every bounded context (Tenancy first, then Identity, Store Brands, Stores, …).
4. Use **Event-driven maturity ladder** to grow async behavior in safe steps.
5. Use **Adapting this playbook** if you apply the same ideas in another stack or greenfield system.

For **step-by-step Tenancy work** (phases, file pointers, checklists, trade-offs), use the operational guides under **[docs/migrations/](migrations/README.md)** — start with [01-tenancy-module.md](migrations/01-tenancy-module.md). This playbook stays **conceptual**; those guides stay **operational**.

**Before module Phase 0:** Centralized MSBuild and packages under `backend/ecommerce-api/` (`Directory.Build.props`, `Directory.Packages.props`, `.editorconfig`) — see [7.1](#71-centralized-build-and-packages-prerequisite).

---

## 1. Introduction

### 1.1 Why migrate?

Vertical slices inside a shared **Application** / **Infrastructure** stack give excellent **feature locality** but weak **system boundaries**. As domains multiply (tenancy, stores, catalog, orders, payments), teams hit:

- Accidental coupling through shared `DbContext` and repositories
- Unclear ownership of tables and business rules
- Painful future extraction to services or teams

A **modular monolith** keeps **one process and one database** (initially) while making **boundaries explicit**: each module owns its domain, application workflows, persistence configuration, and HTTP surface (or subset thereof), and talks to others only through **contracts** and **integration events**.

### 1.2 What we are not doing (yet)

- **Not** a big-bang rewrite.
- **Not** mandating MediatR or a specific bus product.
- **Not** replacing synchronous request/response inside a module with a message queue — commands and queries stay **in-process**.
- **Not** splitting databases until there is a strong operational reason.

### 1.3 What “success” looks like

- Each bounded context is **relocatable**: could become a separate service later with less surgery.
- **Tenant isolation** and **global vs tenant** data rules remain enforceable (ADR-0001, ADR-0002).
- **Vertical slices** remain the internal shape of each module (ADR-0003).
- **Result**-based errors stay consistent at boundaries (ADR-0004).
- **Integration events** exist for cross-module reactions; transport (in-process → outbox → broker) is **swappable**.

---

## 2. Architectural context (ADR map)

| ADR | Short role in migration |
|-----|-------------------------|
| [0001](adr/0001-multi-tenancy-model.md) | Tenant as isolation boundary; membership; informs Tenancy and authorization. |
| [0002](adr/0002-store-brand-vs-store-instance.md) | Global **StoreBrand** vs tenant **StoreInstance** — drives separate modules and schemas. |
| [0003](adr/0003-vertical-slice-architecture.md) | Commands / queries / features **inside** each module — migration does not replace this. |
| [0004](adr/0004-result-pattern.md) | Expected failures as data; consistent API error mapping. |
| [0005](adr/0005-modular-monolith-bounded-context-modules.md) | **Target**: module folders, four layers per module, communication rules, module catalog. |

**Rule of thumb:** Business rules and ADRs override convenience refactors.

---

## 3. Where we started (baseline)

This backend began as (and largely still is) a **layered monolith**:

- **Api** — HTTP, middleware, composition.
- **Application** — vertical slices (e.g. `Admin/Tenants/...`), services, validators, authorization policies.
- **Domain** — aggregates, `TenantEntity`, errors.
- **Infrastructure** — single `EcommerceDbContext`, EF configurations, repositories, tenant resolution implementation.

**Strengths to preserve:** slice-based organization, explicit use cases, EF global filters for tenant-scoped entities, header-based tenant resolution.

**Gaps relative to ADR-0005:** no bounded-context **project** boundaries; all persistence routes through one context; cross-domain code lives in the same assemblies; “module” is a folder concept only.

**Strangler pattern:** introduce **new** module projects and a **host registry**; move slices **incrementally**; shrink legacy projects until they disappear.

---

## 4. Target architecture (summary)

Full detail lives in [ADR-0005](adr/0005-modular-monolith-bounded-context-modules.md). Operational summary:

### 4.1 Module shape (every bounded context)

```text
<Module>/
 ├── <Module>.Api              — composition entry: *Module extension, optional controllers/middleware
 ├── <Module>.Application      — commands, queries, handlers, contracts exposed to other modules
 ├── <Module>.Core             — domain model, invariants, domain errors
 └── <Module>.Infrastructure   — persistence, external adapters owned by this context
```

Inside **Application**, keep vertical slices (ADR-0003).

### 4.2 Host

The **Api** host remains responsible for cross-cutting concerns that are truly global: authentication setup, Serilog, exception handling, health checks, and **calling each module’s registration** (e.g. `ModulesRegistry`). It should **not** accumulate business logic over time.

### 4.3 Shared kernel (cross-cutting, not a business module)

Shared libraries hold **technical** building blocks only, for example:

- In-process **command/query** dispatch (optional alternative to hand-wired `IXxxService` interfaces)
- **Integration event** abstractions and an **in-process** publisher first
- Later: outbox interfaces, serialization helpers — **not** another module’s entities

Avoid turning Shared into a junk drawer for domain types.

### 4.4 Data: one database, multiple DbContexts (incremental)

Until you split databases for operational reasons:

- Each module introduces its own **`DbContext`** that maps **only the tables it owns**.
- **Same connection string** and usually the same schema.
- **Exactly one owning context** per table for migrations (avoid two contexts fighting the same migration history).

Legacy `EcommerceDbContext` **shrinks** as tables migrate; eventually it is deleted.

### 4.5 Communication between modules

| Mechanism | Use for |
|-----------|---------|
| Application **interfaces** and DTOs | Synchronous, in-process collaboration (e.g. “check membership”) |
| **Integration events** + publisher | “Something happened” — other modules react; later outbox or broker |
| **Never** another module’s `DbContext` or internal repository types | Preserves replaceability and testability |

### 4.6 Commands vs integration events (mental model)

- **Commands / queries** — request/response **inside** the process; part of a user request or job step; handlers own transactions for their aggregate boundaries.
- **Integration events** — **facts** published after (or as part of) a successful state change; multiple subscribers; must become **idempotent** when delivery is retried.

Do **not** route normal API commands through Azure Service Bus as if they were MediatR — latency, failure modes, and transaction boundaries differ.

---

## 5. Strategic decisions (frozen for this migration)

These choices reduce debate during implementation:

1. **Tenancy first** — establishes `DbContext` split, module registration, and tenant HTTP surface as a template.
2. **Incremental table ownership** — new module context alongside legacy context until cutover is proven.
3. **Identity second (recommended)** — users and Clerk vs tenant membership rows; clear contracts with Tenancy.
4. **Store Brands (global) before Stores (tenant)** — matches ADR-0002 dependency direction.
5. **Event transport ladder:** in-process publisher → outbox → optional broker — same `IEventPublisher` abstraction where possible.
6. **One phase = one review milestone** — optionally one git commit per phase for a tutorial-clean history.

---

## 6. Per-module phase template (repeat for every module)

Treat each bounded context as a **mini-project** with the same rhythm. Phase names can be tuned; counts may vary (e.g. merge test phase into move phase for small modules).

| Phase | Goal | “Green” means |
|-------|------|----------------|
| **0 — Extend shared kernel** (only when needed) | Add or fix cross-cutting primitives (dispatchers, event interfaces, registration extension). | Solution builds; no regression; existing behavior unchanged. |
| **1 — Scaffold module** | Four projects, solution entries, empty `*Module` with feature flag; references correct; no business move yet. | Builds; app runs unchanged. |
| **2 — Module DbContext** | New context + configurations + migrations for **this** module’s tables only; register in DI. | Builds; app runs; dual-context period acceptable. |
| **3 — Move behavior** | Domain, handlers, repos, HTTP adapters, middleware owned by this context; publish first integration events if applicable. | Feature parity; endpoints behave as before. |
| **4 — Host cleanup** | Host stops registering duplicate services; module owns its slice of pipeline and DI. | No duplicate middleware or duplicate `DbSet` ownership. |
| **5 — Tests** | Tests colocated with module or dedicated test projects; contract tests where needed. | Full test suite green. |
| **6 — Contracts and docs** | Public interfaces and event contracts documented for **downstream** modules. | New developers know what they may call vs what is internal. |

**Anti-patterns to avoid**

- Two modules writing the same aggregate through different contexts without a clear owner.
- “Temporary” direct `DbContext` injection across module boundaries — it becomes permanent.
- Publishing integration events **before** the business transaction commits (outbox solves this later).
- Skipping tests until the end — Phase 3+ becomes unreviewable.

---

## 7. First implementation: Tenancy module (phase outline)

### 7.1 Centralized build and packages (prerequisite)

Complete this **once** for the whole API solution before Tenancy Phase 0. It is not a bounded-context concern, but every new module project inherits these settings automatically.

**Location** (next to `Ecommerce.slnx` and `Dockerfile`):

```text
backend/ecommerce-api/
├── Directory.Build.props      # TargetFramework, nullable, analyzers
├── Directory.Packages.props   # Central Package Management (CPM) — versions here only
├── .editorconfig              # C# style for this solution tree
├── tests/
│   └── Directory.Build.props  # IsPackable=false; imports parent props
└── src/ ...
```

| File | Purpose |
|------|---------|
| `Directory.Build.props` | `net10.0`, nullable, implicit usings, `EnforceCodeStyleInBuild` aligned with `.editorconfig` |
| `Directory.Packages.props` | All `PackageVersion` entries; project `.csproj` files use `<PackageReference Include="..." />` **without** `Version` |
| `.editorconfig` | C# conventions under `backend/ecommerce-api/` only (frontend keeps its own tooling at repo root) |
| `tests/Directory.Build.props` | Marks test projects non-packable; **must import** parent `Directory.Build.props` (nested file otherwise hides `TargetFramework`) |

**When adding a new module** (e.g. `Ecommerce.Tenancy.Infrastructure`):

1. Create the `.csproj` with `ProjectReference` / `PackageReference` only — no version, no duplicate `TargetFramework`.
2. Add any **new** NuGet packages to `Directory.Packages.props` first.
3. Run `dotnet build` on `Ecommerce.slnx`.

**Trade-off:** CPM requires discipline (one place for versions) but prevents drift across ten+ projects during migration.

---

Tenancy is the **reference migration**: it touches HTTP (`X-Tenant-Id`), scoped context, EF filters on tenant-scoped entities elsewhere, and membership-related authorization.

**Phase alignment** (seven phases = seven review milestones; see companion plan for detail):

| Phase | Focus |
|-------|--------|
| 0 | Shared: `IIntegrationEvent`, `IEventPublisher`, in-process publisher, dispatcher registration, fix query dispatcher if needed. |
| 1 | `Tenancy.Core` / `.Application` / `.Infrastructure` / `.Api` scaffold + solution wiring. |
| 2 | `TenancyDbContext` for tenant tables; coexist with legacy context. |
| 3 | Move slices: domain, handlers, repos, controllers, middleware; `TenantOnboarded`-style event from onboarding. |
| 4 | `TenancyModule` owns `AddTenancy`-equivalent and pipeline pieces; remove legacy duplication; fix authorization handler registration gaps. |
| 5 | Relocate and add tests. |
| 6 | Document `ITenantContext` and integration events for Identity and Stores consumers. |

After Tenancy, **copy the template** — do not redesign from scratch each time.

---

## 8. Incremental migration guide (principles)

### 8.1 Strangler fig inside one repo

- Add **new** structure beside **old** code.
- Route traffic feature-flag or module-flag if needed (optional).
- Delete old code only when tests prove parity.

### 8.2 Dependency direction

- **Core** depends on nothing outside domain primitives.
- **Application** depends on Core + shared abstractions + **outward** contracts (interfaces), not concrete Infrastructure.
- **Infrastructure** implements Application ports and hosts EF.
- **Api** wires framework concerns and calls `AddXxxModule`.

### 8.3 Global vs tenant modules

- **Global modules** (e.g. Store Brands): no `TenantId`; APIs may not require tenant header.
- **Tenant modules** (e.g. Tenancy, Stores, most commerce): require tenant context and enforce isolation in handlers, auth, and EF filters.

Classify each module **before** writing APIs to avoid leaking tenant scope into global domains.

### 8.4 Authorization at boundaries

Policies that need both **user identity** and **tenant membership** span modules conceptually. Prefer **small interfaces** on the Tenancy side (e.g. membership checks) invoked from the host or Identity-related registration — avoid cyclic project references.

### 8.5 Migrations and schema ownership

When splitting `DbContext`:

- Pick **one** context to own migrations for a given table family.
- Use EF tooling against the **owning** context.
- After cutover, remove entities from the legacy context to prevent double mapping.

---

## 9. Event-driven maturity ladder

Use this as a **learning path** inside the modular monolith:

| Step | Capability | Risk if skipped |
|------|------------|-----------------|
| 1 | Define **integration event** types and **in-process** publisher + handlers | None — learning only |
| 2 | Multiple handlers per event (e.g. log + future notification) | Coupling if you call other modules’ services directly instead |
| 3 | **Transactional outbox** — persist event with same transaction as aggregate change | Lost events on crash or partial failure |
| 4 | **Outbox dispatcher** — background delivery to in-process or bus | Manual “fire and forget” `Task.Run` anti-pattern |
| 5 | **Idempotent** consumers (`EventId` dedup) | Double processing when retries exist |
| 6 | Swap publisher to **Azure Service Bus**, **RabbitMQ**, etc. | Premature if 3–5 not understood |

**Azure Event Grid** is often orthogonal: external fan-out and cloud integrations, not a replacement for your command path or internal integration pipeline.

---

## 10. Roadmap after Tenancy (high level)

Order is intentional; each item repeats the **per-module phase template**.

1. **Identity** — users, external IdP boundary, contracts with Tenancy for membership checks.
2. **Store Brands** — global brand catalog.
3. **Stores** — `StoreInstance` as tenant operational unit; first major **new** API surface on existing schema.
4. **Commerce modules** (Catalog → Inventory → Pricing → Cart → Orders → Payments → Fulfillment) — per ADR-0005 catalog; dependencies flow from Stores downward.
5. **Supporting modules** — Notifications (event subscribers), Reporting (read models), Search (optional).
6. **Retire legacy** `Ecommerce.Application` / `Domain` / `Infrastructure` when empty; move shared primitives (e.g. `Result`) to Shared or a tiny kernel package.
7. **Optional service extraction** — per bounded context when scale or team boundaries demand it; integration events become network messages.

---

## 11. Adapting this playbook

### 11.1 Another .NET codebase

- Keep ADR-0005-equivalent **written** before large moves.
- Reuse the **phase template**; rename projects to your namespaces.
- If you already use MediatR, you may **skip** custom dispatchers or wrap MediatR behind your module’s facade — consistency matters more than dogma.

### 11.2 Another language or framework

- Map **Application / Core / Infrastructure / Api** to your ecosystem’s idioms.
- Preserve: **single write model per aggregate**, **explicit module boundaries**, **integration events for cross-context facts**, **incremental Db ownership** if monolith DB first.

### 11.3 Greenfield modular monolith

- Start with **one module** and Shared kernel; add modules as domains appear — easier than migrating later.
- Still use **integration events early** with in-process publisher to avoid “big rewrite to events” later.

---

## 12. Related documents

| Document | Role |
|----------|------|
| [ADR-0005](adr/0005-modular-monolith-bounded-context-modules.md) | Canonical target architecture and module list. |
| [ADR-0001](adr/0001-multi-tenancy-model.md) / [ADR-0002](adr/0002-store-brand-vs-store-instance.md) | Tenancy and store modeling constraints. |
| [ADR-0003](adr/0003-vertical-slice-architecture.md) | Inside-module structure. |
| [ADR-0004](adr/0004-result-pattern.md) | Error model at boundaries. |
| [Business rules](business-rules.md) | Rules T-*, S-*, SR-* that modules must enforce. |
| [Tenancy module migration](migrations/01-tenancy-module.md) | Operational phases 0–6, file map, checklists, trade-offs. |
| [Migrations index](migrations/README.md) | Ordered list of module guides (Identity, Store Brands, … planned). |
| [.NET Aspire orchestration (optional)](platform/dotnet-aspire-orchestration.md) | Local dev orchestration — apply after module migration, not a migration phase. |
| [OpenTelemetry observability (optional)](platform/opentelemetry-observability.md) | Traces/metrics alongside Serilog — apply after migration when you choose a backend. |

---

## 13. Glossary (quick)

| Term | Meaning here |
|------|----------------|
| **Bounded context** | A coherent domain model with explicit boundaries (a “module” in ADR-0005). |
| **Modular monolith** | Single deployable unit composed of modules with strict boundaries. |
| **Integration event** | A fact published for other contexts to react — stable payload, versioning considerations later. |
| **Outbox** | Store events in the same DB transaction as domain changes; process asynchronously for reliability. |
| **Strangler fig** | Incrementally replace legacy paths until legacy can be removed. |
| **Vertical slice** | Feature organized by use case (command/query) rather than only by technical layer. |

---

## 14. Platform extras (optional — not migration phases)

Module migration (Tenancy → Identity → …) does **not** depend on these. Adopt them **after** the modular monolith structure is stable, or when local/dev pain justifies the investment.

### Local orchestration (.NET Aspire)

**Purpose:** Run the API, PostgreSQL, and future workers (outbox processor, search, etc.) from one local command with a dashboard and consistent connection strings.

**When it helps:** Multiple processes locally; several developers; you add background workers or extra containers.

**When to skip for now:** Single API + one Postgres on Railway; `dotnet run` + Docker Compose already works.

**Setup guide:** [docs/platform/dotnet-aspire-orchestration.md](platform/dotnet-aspire-orchestration.md)

### Observability (OpenTelemetry + Serilog)

**Purpose:** **Serilog** remains structured logging (Railway-friendly stdout today). **OpenTelemetry** adds vendor-neutral **traces** and **metrics**; export to Azure Monitor, Grafana Cloud, Jaeger, or OTLP — Azure is optional, not required.

**When it helps:** Debugging cross-module requests, measuring handler latency, preparing for production APM.

**When to skip for now:** Migration is the priority; Railway console logs are enough for solo dev.

**Setup guide:** [docs/platform/opentelemetry-observability.md](platform/opentelemetry-observability.md)

---

*Last updated: aligns with ADR-0005 and the Tenancy seven-phase migration approach used in this repository.*
