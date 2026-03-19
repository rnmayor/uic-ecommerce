using Ecommerce.Application.Admin.Tenants.Queries.GetMyTenants;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Tenants;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Repositories.Tenants.Queries.GetTenantsForUser
{
    public sealed class GetTenantsForUserRepository : IGetTenantsForUserRepository
    {
        private readonly EcommerceDbContext _context;
        public GetTenantsForUserRepository(EcommerceDbContext context)
        {
            _context = context;
        }
        public async Task<IReadOnlyList<MyTenantDTO>> GetTenantsForUserAsync(Guid userId, CancellationToken ct = default)
        {
            return await _context.TenantUsers
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(tu => tu.UserId == userId)
                .Select(tu => new MyTenantDTO
                {
                    TenantId = tu.TenantId,
                    Name = tu.Tenant.Name,
                    IsOwner = tu.Role == TenantRoles.Owner,
                    Role = tu.Role
                })
                .ToListAsync(ct);
        }
    }
}