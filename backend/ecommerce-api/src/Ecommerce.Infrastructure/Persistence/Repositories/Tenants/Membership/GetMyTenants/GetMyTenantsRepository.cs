using Ecommerce.Application.Admin.Tenants.Membership.GetMyTenants;
using Ecommerce.Domain.Tenants;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Repositories.Tenants.Membership.GetMyTenants;

public sealed class GetMyTenantsRepository : IGetMyTenantsRepository
{
    private readonly EcommerceDbContext _context;
    public GetMyTenantsRepository(EcommerceDbContext context)
    {
        _context = context;
    }
    public async Task<IReadOnlyList<MyTenantDto>> GetTenantsForUserAsync(Guid userId, CancellationToken ct = default)
    {
        return await _context.TenantUsers
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(tu => tu.UserId == userId)
            .Select(tu => new MyTenantDto
            {
                TenantId = tu.TenantId,
                Name = tu.Tenant.Name,
                IsOwner = tu.Role == TenantRoles.Owner,
                Role = tu.Role
            })
            .ToListAsync(ct);
    }
}
