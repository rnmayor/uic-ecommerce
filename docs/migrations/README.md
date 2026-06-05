# Module migration guides

Operational, phase-by-phase guides for extracting bounded contexts from the legacy layered stack into ADR-0005 modules. Use these while you implement; pair with the conceptual playbook.

| Order | Guide | Status |
|-------|--------|--------|
| — | [Migration playbook (concepts)](../migration-playbook-from-layered-monolith-to-modular-monolith.md) | Living |
| 1 | [Tenancy module](01-tenancy-module.md) | In progress |
| 2 | Identity module | Planned |
| 3 | Store Brands module | Planned |
| 4 | Stores module | Planned |

**Before Phase 0 (Tenancy):** Ensure [centralized build and packages](../migration-playbook-from-layered-monolith-to-modular-monolith.md#71-centralized-build-and-packages-prerequisite) exist under `backend/ecommerce-api/` (`Directory.Build.props`, `Directory.Packages.props`, `.editorconfig`).

**How to work:** one phase per review milestone (optionally one git commit per phase). After each phase: `dotnet build`, run tests, then ask for mentor review with errors or “Phase N done.”

Future guides (`02-identity-module.md`, etc.) will reuse the same phase template documented in the playbook.

**Platform (optional, after migration):** Local orchestration and observability are **not** module phases. See [docs/platform/](../platform/) — [.NET Aspire orchestration](../platform/dotnet-aspire-orchestration.md) and [OpenTelemetry observability](../platform/opentelemetry-observability.md).
