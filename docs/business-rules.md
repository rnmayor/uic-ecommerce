## Business Rules

### Core Concepts

1. Tenant

- A Tenant represents an organizational boundary.
- Examples: Public Market, Mall, Business park, Company operating multiple stores
- All authorization, isolation, and data ownership are tenant-scoped.

2. User

- A User is an authenticated identity (Clerk).
- A user may:
  - Belong to zero or more tenants
  - Have different roles per tenant
- Users are never tenant-scoped.

3. TenantUser (Membership)

- A TenantUser represents membership + role.
- Every tenant has at least one TenantUser.
- Roles apply only within a tenant.

4. StoreBrand

- A StoreBrand represents a merchant identity.
- Examples: Shawarma King, 7-Eleven, SM Supermarket
- StoreBrand:
  - Is global
  - Has no TenantId
  - May exist in multiple tenants

5. StoreInstance

- A StoreInstance represents a physical or operational presence of a StoreBrand inside a tenant.
- Examples: “Shawarma King – Gate A”, “Shawarma King – Food Court”
- StoreInstance:
  - Belongs to one tenant
  - Belongs to one StoreBrand
  - Has its own inventory, orders, pricing

6. Tenant Onboarding Rules

- _Rule T-01 — Tenant discovery after login_
- After login, system checks tenant memberships.
- If user has zero tenants, onboarding is required.

- _Rule T-02 — Creating a tenant_
- Any authenticated user may create exactly one tenant they own.
- Creating a tenant:
  - Creates a Tenant
  - Creates a TenantUser with role Owner
- No stores are created automatically.

- _Rule T-03 — Tenant ownership_
- A tenant has exactly one Owner at all times.
- Ownership transfer:
  - Must be explicit
  - Must be atomic
  - Old owner loses Owner role

- _Rule T-04 — Tenant deletion_
- Only the Owner may delete a tenant.
- Deleting a tenant deletes:
  - TenantUsers
  - StoreInstances
  - Tenant-scoped data
- StoreBrands are never deleted automatically.

7. Membership & Invitations

- _Rule M-01 — Membership model_
- A user may belong to multiple tenants.
- A user may have different roles per tenant.
- A tenant may have multiple users.

- _Rule M-02 — Inviting users_
- Only Owner or Admin may invite users.
- Invitations are tenant-scoped.
- Invitations define:
  - Tenant
  - Role
- Accepting an invite creates a TenantUser.

- _Rule M-03 — Joining a tenant_
- Users cannot self-join tenants.
- Joining requires:
  - Invite link
  - Or explicit approval workflow (future)

- _Rule M-04 — Removing members_
- Owner/Admin may remove members.
- Owner cannot remove themselves unless ownership is transferred first.

8. Roles & Authorization Rules

- Canonical Roles: Owner, Admin, Manager, Staff, Customer
- Role meanings (minimum guarantees):
- _Owner_
  - Full control
  - Ownership transfer
  - Billing
  - Tenant deletion

- _Admin_
  - Tenant configuration
  - User management
  - Store creation
  - Pricing, inventory

- _Manager_
  - Catalog management
  - Inventory
  - Orders

- _Staff_
  - Order fulfillment
  - Read-only catalog
  - POS-style workflows

- _Customer_
  - Checkout
  - Order history
  - No admin access

- _Rule R-01 — Policies > Roles_
- Authorization uses policies, not role checks.
- Policies describe capability, not title.
- Roles are mapped to policies.

9. Store Rules

- _Rule S-01 — Store creation_
- Stores are never created during tenant onboarding.
- Stores are created explicitly after tenant selection.

- _Rule S-02 — StoreBrand behavior_
- StoreBrands are:
  - Global
  - Deduplicated by name
- Creating a store:
  - Finds or creates StoreBrand

- _Rule S-03 — StoreInstance behavior_
- Creating a store:
  - Creates a StoreInstance for the current tenant
- StoreInstance:
  - Has inventory
  - Has pricing
  - Has orders
  - Is tenant-scoped

- _Rule S-04 — Multiple instances per tenant (flexible model)_
- A tenant may have multiple StoreInstances for the same StoreBrand.
- DisplayName distinguishes instances.
- Example: Shawarma King – Gate A, Shawarma King – Gate C

- _Rule S-05 — Store deletion_
- Deleting a StoreInstance:
  - Does NOT delete StoreBrand
- Deleting a StoreBrand:
  - Only allowed if no StoreInstances exist
  - Otherwise restricted

10. Tenant Resolution & Security Rules

- _Rule SR-01 — Tenant resolution_
  - All authenticated admin endpoints require a tenant context.
  - Tenant context comes from: X-Tenant-Id header

- _Rule SR-02 — Explicit exceptions_
- Endpoints may explicitly opt out via: [SkipTenantResolution]
- Used for:
  - Login callbacks
  - Tenant discovery
  - Tenant onboarding

- _Rule SR-03 — Query isolation_
- All TenantEntity models:
  - Are protected by global query filters
- Cross-tenant data leakage is impossible by default.

11. Data Integrity Rules (DB-level)

- Enforced constraints
  - One Owner per tenant
  - User cannot join the same tenant twice
  - StoreInstance always belongs to:
    - One Tenant
    - One StoreBrand
  - StoreBrand cannot be deleted if in use

12. UX Flow Summary

- _First-time user_
- Login
- No tenants found
- Must create or join tenant
- Tenant created → Owner assigned
- User explicitly adds stores

- _Returning user_
- Login
- Tenant selector shown
- Tenant context established
- Admin or marketplace flow continues
