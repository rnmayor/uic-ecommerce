# ADR-0005: Modular Monolith with Bounded Context Modules

## Status

Accepted

## Context

As the platform evolves into a large-scale, multi-tenant e-commerce system, the existing structure based on Vertical Slice Architecture (ADR-0003) provides strong feature organization but lacks explicit system-level boundaries.

The system must support:

- Clear separation of business domains (tenancy, stores, commerce, etc.)
- Independent evolution of features
- Strong tenant isolation (ADR-0001)
- Proper modeling of global vs tenant-scoped entities (ADR-0002)
- Scalable development across multiple teams
- Future transition to microservices without major refactoring

Additionally, the domain includes critical distinctions:

- Global entities (e.g., StoreBrand)
- Tenant-scoped entities (e.g., StoreInstance, Orders, Inventory)
- Cross-cutting concerns (Identity, Authorization)

Without explicit modular boundaries:

- Domain responsibilities become blurred
- Cross-domain coupling increases
- Scaling the system becomes difficult
- Extracting services later becomes risky and expensive

## Decision

We adopt a Modular Monolith Architecture, where the system is composed of bounded context modules.

1. Module Definition:

A module represents a bounded context that:

- Encapsulates a specific business domain
- Owns its data and behavior
- Enforces its own invariants
- Exposes functionality via explicit contracts

Each module is independently structured with its own layers:

```
<Module>/
 ├── <Module>.Api
 ├── <Module>.Application
 ├── <Module>.Core
 └── <Module>.Infrastructure
```

2. Internal Structure (Relation to ADR-0003)

Inside each module:

- We continue using Vertical Slice Architecture
- Features are organized into:
  - Commands
  - Queries
  - Feature workflows

ADR-0003 remains fully valid within each module.

3. Module Communication

Modules interact via:

- Application contracts (interfaces, DTOs)
- Explicit method calls across module boundaries
- Domain / integration events (future)

Direct access to another module’s internal implementation is not allowed.

4. Global vs Tenant Domain Separation

The architecture explicitly enforces:

- Global modules
  - No TenantId
  - Shared across the system

- Tenant-scoped modules
  - Require tenant context
  - Enforce isolation via:
    - Application logic
    - Authorization
    - Data access filters

This aligns with:

- ADR-0001 (Tenant model)
- ADR-0002 (StoreBrand vs StoreInstance)

5. Defined Modules

### CORE PLATFORM MODULES (FOUNDATION)

1. Identity / Access

- Users (external via Clerk, internal representation)
- Authentication context
- Roles / Permissions (policy-based)
- Tenant membership (TenantUser)
- Invitations

Why separate:

- Cross-cutting across all modules
- External IdP boundary
- Centralized authorization (Rule R-01)

2. Tenancy / Accounts

- Tenant
- Tenant lifecycle (create, delete, transfer ownership)
- Onboarding
- Tenant discovery / selection
- Subscription / Plan (future)

Why separate:

- System root boundary
- Enforces:
  - Tenant isolation (ADR-0001)
  - Ownership invariants
  - Onboarding rules (T-01 → T-04)

3. Stores (Tenant-scoped)

- StoreInstance
- Store management
- Store configuration

Why separate:

- Operational unit of the business
- Tenant-scoped
- Enforces Rule S-01 → S-05

4. Store Brands (Global)

- StoreBrand
- Brand deduplication
- Brand management

Why separate:

- Global domain (no TenantId)
- Shared across tenants
- Required by ADR-0002

### COMMERCE DOMAIN MODULES

5. Catalog

- Products, variants, categories, attributes, media

Why separate:

- High read volume
- Pure product definition
- No stock or pricing concerns

6. Inventory

- Stock levels
- Reservations
- Adjustments
- Warehousing (future)

Why separate:

- Strong consistency boundary
- Concurrency-critical
- Must not be coupled with Catalog

7. Pricing

- Price lists
- Discounts
- Promotions
- Campaign rules

Why separate:

- Highly dynamic
- Independent business logic

8. Cart

- User cart
- Session cart
- Cart state

Why separate:

- Transitional state
- Not a strong aggregate
- May evolve to distributed caching

9. Orders

- Order aggregate
- Checkout
- Order lifecycle

Why separate:

- Core transactional boundary
- Central to business operations

10. Payments

- Payment intents
- Payment processing
- Gateway integration

Why separate:

- External system boundary
- Failure isolation
- Async workflows

11. Fulfillment / Shipping

- Delivery methods
- Shipment tracking
- Fulfillment workflows

Why separate:

- Operational domain
- Third-party integrations

### SUPPORTING MODULES

12. Reporting

- Read models
- Aggregations
- Analytics

Why separate:

- CQRS read side
- No domain logic

13. Notifications

- Email, SMS, Webhooks
- Domain event consumers

Why separate:

- Event-driven
- Cross-cutting
- Asynchronous

14. Search (Optional)

- Full-text search
- Indexing (Meilisearch / Elasticsearch)

Why separate:

- Infrastructure-heavy
- Eventually consistent

### System Structure Overview

```
PLATFORM
 ├── Identity
 ├── Tenancy
 ├── Store Brands (GLOBAL)
 └── Stores (TENANT)

COMMERCE
 ├── Catalog
 ├── Inventory
 ├── Pricing
 ├── Cart
 ├── Orders
 ├── Payments
 └── Fulfillment

SUPPORT
 ├── Reporting
 ├── Notifications
 └── Search
```

## Consequences

Positive

- Clear separation of concerns at system level
- Strong alignment with business domain
- Reduced coupling between modules
- Enables parallel development
- Simplifies future microservice extraction
- Improves maintainability and scalability

Trade-offs

- Increased number of projects
- More explicit boundaries require discipline
- Cross-module communication must be carefully managed
- Initial setup complexity is higher than layered architecture

## Notes

- Modules are designed for future service extraction
- No module should directly access another module’s persistence layer
- Global vs Tenant boundaries must always be respected
- Feature flags and subscriptions can be layered on top of modules
- Relationship to Other ADRs:
  - ADR-0001 (Multi-Tenancy Model)
    - Defines tenant boundary enforced by modules

  - ADR-0002 (StoreBrand vs StoreInstance)
    - Directly reflected in separate modules

  - ADR-0003 (Vertical Slice Architecture)
    - Applied within each module

  - ADR-0004 (Result Pattern)
    - Used across modules for consistent error handling
