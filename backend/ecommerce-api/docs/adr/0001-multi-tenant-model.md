# ADR-0001: Multi-Tenant Model

## Status

Accepted

## Context

The platform is a multi-tenant system where multiple organizations operate
within a shared application instance.

Key requirements:

- Strong data isolation between organizations
- Users may belong to multiple organizations
- All business data must be scoped to a tenant
- Authorization must be enforceable at the tenant boundary

Early design decisions are required to define:

- What constitutes a Tenant
- How users relate to tenants
- How tenant context is resolved and enforced

These decisions form the foundation for all domain modeling.

## Decision

We model **Tenant** as the primary isolation and ownership boundary in the system.

### Tenant

- Represents a real-world organization or business entity
- Owns all business data created within its context
- Is required for all write and most read operations

### Tenant Membership

- Users do not own data directly
- Users access the system _only_ through TenantMembership
- A user may belong to multiple tenants
- Membership defines the user’s role within that tenant

### Ownership Invariant

- Every Tenant must have exactly one Owner
- Ownership is established at tenant creation
- The Owner cannot be removed while the tenant exists

### Tenant Resolution

- Each request must resolve a Tenant context
- If no tenant is resolved:
  - The user is prompted to create or join a tenant
- Tenant context is enforced at:
  - Application services
  - Authorization checks
  - Data access boundaries

### Data Isolation

- All tenant-scoped entities must reference `TenantId`
- Data belonging to one tenant must never be visible or accessible to another tenant
- Entities that are shared across tenants must be explicitly modeled as global (not tenant-scoped)

## Alternatives Considered

### Single-tenant with organizations represented as labels

Rejected because:

- No strong isolation guarantees
- Authorization becomes error-prone
- High risk of data leakage

### One user = one tenant

Rejected because:

- Prevents collaboration
- Does not reflect real organizational structure

### One store = one tenant

Rejected because:

- Prevents modeling multi-store organizations
- Forces duplication of tenant-level configuration and users
- Does not support franchises, malls, or large tenants with multiple store instances

## Consequences

Positive:

- Clear ownership and isolation model
- Predictable authorization rules
- Scales naturally to enterprise use cases
- Enables flexible onboarding and invitations

Trade-offs:

- Requires explicit tenant resolution
- Slightly more complexity in request handling
- All domain models must respect tenant scoping

## Notes

- Tenant-scoped entities must include `TenantId`
- Global entities must not reference TenantId
- Future features (stores, orders, inventory) must align with this model
