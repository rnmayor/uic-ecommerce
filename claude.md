# Claude Guidelines for uic-ecommerce

## Purpose

Use this document as the Claude prompt guide for working on the `uic-ecommerce` repository. It aligns with the repository’s architectural rules, ADRs, migration playbook, and platform direction.

## Identity & Mission

This repository contains a portfolio-grade, enterprise-style, multi-tenant SaaS e-commerce platform.

The platform serves as a long-term technical demonstration of:

- Domain-Driven Design (DDD)
- Vertical Slice Architecture
- Modular architecture with bounded contexts
- Event-driven design and asynchronous workflows
- Multi-tenancy and data isolation
- Clean Architecture principles
- Modern .NET backend development
- Angular enterprise application development
- Next.js marketplace architecture
- Cloud-native deployment, observability, and operational excellence

The architecture follows an evolutionary approach. Modules may remain inside a modular monolith or be extracted into independently deployable services when justified by business, operational, or scaling requirements.

Prioritize production-grade engineering quality at all times:

- Maintainability over cleverness
- Explicit design over implicit behavior
- Type safety over convenience
- Testability and observability by default
- Strict architectural boundaries
- No placeholder or pseudo code
- Ask for clarification instead of making assumptions

## Rule Priority

When rules conflict, apply this order:

1. Business Rules Document
2. ADR Documents
3. Architecture & Dependency Rules
4. Coding Standards
5. Style / Formatting

## Core Architecture Anchors

- ADR-0001: Multi-tenant model and isolation boundary
- ADR-0002: StoreBrand (global) vs StoreInstance (tenant-scoped)
- ADR-0003: Vertical Slice Architecture inside modules
- ADR-0004: Result Pattern for error handling
- ADR-0005: Modular Monolith with bounded context modules

## Migration Playbook Alignment

- Follow `docs/migration-playbook-from-layered-monolith-to-modular-monolith.md` as the conceptual roadmap.
- Use the per-module phase template described in the playbook when extracting bounded contexts.
- Start with Tenancy first, then Identity, Store Brands, Stores, Catalog, Inventory, Pricing, Cart, Orders, Payments, Fulfillment.
- Use the event-driven maturity ladder:
  - in-process publisher first
  - then outbox persistence
  - then optional external broker
- Do not treat integration events as a substitute for normal synchronous command/query boundaries.

## ADR-0005 and Module Rules

- Each bounded context is a module with a four-layer structure:
  - `<Module>.Api`
  - `<Module>.Application`
  - `<Module>.Core`
  - `<Module>.Infrastructure`
- Modules interact only through explicit contracts and public application interfaces.
- Do not access another module’s internal implementation, repository, or DbContext directly.
- Global modules must not depend on tenant-scoped data.
- Tenant-scoped modules must enforce tenant isolation using application logic, authorization, and EF filters.
- The shared kernel may contain technical abstractions only, not domain entities or business logic.

## Platform Direction

- Use `docs/platform/README.md` for optional platform guides after the modular monolith is stable.
- `docs/platform/dotnet-aspire-orchestration.md` is for local orchestration of API + Postgres + workers.
- `docs/platform/opentelemetry-observability.md` is for adding traces/metrics alongside Serilog.
- Platform extras are not part of the core migration; do them only after ADR-0005 architecture is in a stable state.

## Backend Stack Summary

- .NET 10, ASP.NET Core Web API
- PostgreSQL with EF Core
- Central Package Management via `Directory.Packages.props`
- xUnit + FluentAssertions
- Single solution host with modular projects registered by module extension methods

## Frontend Summary

- Admin: Angular 21, Signals, TanStack Query, Zod, Angular Material + TailwindCSS
- Marketplace: Next.js 16 App Router, React Server Components, Zod, shadcn/ui, TailwindCSS
- Testing: Vitest, co-located tests, use msw for API mocking

## Coding & Architecture Guidance

- Prefer explicit over implicit implementation.
- Do not use direct DbContext access outside Infrastructure.
- Do not put business logic in controllers beyond orchestration.
- Use Result<T> / Result for failures; map to ProblemDetails at API boundaries.
- Keep module registration and DI composition small and explicit.
- Prefer evolutionary architecture over premature distribution.
- Extract services only when justified by clear business, operational, or scaling requirements.

## Output Expectations for Claude

- Return production-ready code that compiles and follows existing repo conventions.
- Avoid fake or incomplete implementations.
- If the task requires design decisions, explain them with reference to ADR-0005 and the migration playbook.
- When writing docs or architecture notes, align with `docs/adr/0005-modular-monolith-bounded-context-modules.md`, `docs/migration-playbook-from-layered-monolith-to-modular-monolith.md`, and `docs/platform/README.md` as the source of truth.
- Prefer solutions that preserve future module extraction capability without introducing distributed-system complexity prematurely.
