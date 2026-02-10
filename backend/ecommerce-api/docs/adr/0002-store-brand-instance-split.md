# ADR-0002: Split Store into StoreBrand and StoreInstance

## Status

Accepted

## Context

The system originally modeled a `Store` as a tenant-scoped aggregate.
This design assumed a one-to-one relationship between a store and a tenant.

New requirements introduced the need for:

- A single brand (e.g. "Nike") to exist in multiple tenants
- A tenant to host multiple stores, potentially from the same brand
- Independent inventory, pricing, and operations per tenant

The existing model could not represent this without duplication or data inconsistency.

## Decision

We split the `Store` concept into two distinct domain models:

- **StoreBrand**
  - Global, tenant-agnostic
  - Represents a brand identity (name, branding, shared catalog)
- **StoreInstance**
  - Tenant-scoped
  - Represents the placement of a StoreBrand within a tenant
  - Owns tenant-specific data such as inventory, pricing, and operations

Tenant onboarding no longer creates a store implicitly.
Stores are created explicitly after tenant creation.

## Alternatives Considered

### Option A: Keep Store tenant-scoped

Rejected because:

- Brands could not be shared across tenants
- Led to data duplication and complex synchronization

### Option B: Brand as value object inside Store

Rejected because:

- Prevented reuse across tenants
- Limited future extensibility (brand-level features, reporting)

## Consequences

Positive:

- Supports multi-tenant brand reuse
- Clear separation of global vs tenant-scoped data
- Aligns with real-world business concepts
- Enables future expansion (franchising, analytics, brand management)

Trade-offs:

- Increased modeling complexity
- Requires explicit store creation after tenant onboarding
- Additional joins when querying store data

## Notes

- A tenant may have multiple StoreInstances for the same StoreBrand
- StoreBrand deletion is restricted while StoreInstances exist
