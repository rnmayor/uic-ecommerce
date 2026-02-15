# ADR-0003: Application Structure - Vertical Slice and DDD aggregates

## Status

Accepted

## Context

As the platform grows, we need a consistent way to organize Application, Infrastructure, Domain, and API layers.

Without clear structure, it is difficult to:

- Navigate code in large teams
- Maintain feature boundaries
- Implement isolated testing
- Extend features without affecting unrelated areas

## Decision

We adopt `Vertical Slice Architecture` combined with `DDD aggregates` modeling:

1. Application Layer

- Organized by features/use-cases (vertical slices) rather than technical types (DTOs, Services)
- Each feature folder contains:
  - Request/Response DTOs
  - Service interface and implementation
- Example:

  ```
  └── 📁Ecommerce.Application
      └── 📁Admin
          └── 📁Stores
              └── 📁Brands
                  └── 📁GetAll
                      ├── GetAllStoreBrandsService.cs
                      ├── IGetAllStoreBrandService.cs
                      ├── StoreBrandDto.cs
                      ├── StoreBrandsResponse.cs
  ```

2. Infrastructure layer

- Mirrors Application layer per feature for repositories
- Aggregate repositories live under Repositories/{Aggregate}
- Example:

  ```
  └── 📁Ecommerce.Infrastructure
      └── 📁Persistence
          └── 📁Repositories
              └── 📁Stores
                  └── 📁Brands
                      └── 📁GetAll
                          ├── GetAllStoreBrandsRepository.cs
              └── 📁Tenants
                  └── 📁Membership
                      └── 📁GetMyTenants
                          ├── GetMyTenantsRepository.cs
                  ├── TenantRepository.cs
  ```

3. Domain Layer

- Contains aggregate roots, entities, value objects, enums, domain exceptions.
- No application or persistence concerns leak into Domain.

4. API layer

- Controllers are feature-scoped and ideally follow the "One Controller Per Feature" pattern (Controllers/Admin/Tenants/OnboardTenantController.cs)
- Validation, middleware, and extensions are organized for cross-cutting concerns.
- Naming: All entry-point methods in controllers are named HandleAsync. The HTTP Verb ([HttpGet], [HttpPost], etc) provides the context of the action.

5. Testing

- Unit and integration testings mirror the vertical slice structure.
- Each use-case folder in Application and API has corresponding tests in Ecommerce.Application.Tests and Ecommerce.Api.Tests

6. Read vs Write Separation

- Read repositories and DTO for queries live per use-case (IGetMyTenantsRepository)
- Aggregate write repositories live under aggregate folder (ITenantRepository) with core operations.
- Naming for Repositories: Standard CRUD naming is used (GetAllAsync, GetByIdAsync, CreateAsync, UpdateAsync, DeleteAsync).

7. Application Service Naming

- To distinguish between intent (Queries vs Commands), application services follow strict naming convention:
- Queries (GET): Use HandleAsync - implies the service is processing a request for data.
- Commands (POST/PUT/DELETE): Use ExecuteAsync - implies the service is performing an action or state change in the system.

## Alternatives Considered

### Traditional layered architecture (Controllers -> Services -> Repositories)

Rejected because:

- Leads to horizontal coupling and CRUD-centric design, harder to scale by feature.

### Single-folder per aggregate with all use-cases inside

Rejected because:

- Harder to maintain and test large aggregates

## Consequences

Positive:

- Clear feature boundaries
- Easier onboarding for new developers
- Encourages aggregate root modeling and DDD practices
- Tests are easier to isolate and maintain
- Application services are decoupled from persistence implementation

Trade-offs:

- More folders/files, slightly more verbose project structure
- Requires discipline in maintaining vertical slices
- Some repetition of DTOs per use-case

## Notes

- Use-case specific repository interfaces is defined inside the use-case namespace and folder
- Aggregate repositories live under Repositories/{Aggregate} in Infrastructure.
