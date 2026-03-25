using Ecommerce.Application.Admin.Tenants.Queries.GetTenant;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Repositories.Tenants.Queries.GetTenant
{
    public sealed class GetTenantRepository : IGetTenantRepository
    {
        private readonly EcommerceDbContext _context;
        public GetTenantRepository(EcommerceDbContext context)
        {
            _context = context;
        }

        public async Task<GetTenantResponse?> GetTenantAsync(Guid tenantId, CancellationToken ct = default)
        {
            var tenant = await _context.Tenants
                .AsNoTracking()
                .Where(t => t.Id == tenantId)
                .Select(t => new GetTenantDTO
                {
                    Id = t.Id,
                    Name = t.Name,
                    Slug = t.Slug
                })
                .FirstOrDefaultAsync(ct);

            if (tenant is null) return null;

            var stores = await _context.StoreInstances
                .AsNoTracking()
                .Where(si => si.TenantId == tenant.Id)
                .Select(si => new GetTenantStoreDTO
                {
                    Id = si.Id,
                    DisplayName = si.DisplayName,
                    StoreBrandName = si.StoreBrand.Name
                })
                .ToListAsync(ct);

            return new GetTenantResponse
            {
                Tenant = tenant,
                Stores = stores
            };
        }
    }
}
