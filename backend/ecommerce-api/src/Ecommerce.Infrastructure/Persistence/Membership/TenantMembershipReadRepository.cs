using Ecommerce.Application.Admin.Tenants.Membership;
using Ecommerce.Domain.Tenants;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Membership;

public sealed class TenantMembershipReadRepository : ITenantMembershipReadRepository
{
    private readonly EcommerceDbContext _context;
    public TenantMembershipReadRepository(EcommerceDbContext context)
    {
        _context = context;
    }
    public async Task<List<MyTenantDto>> GetTenantsForUserAsync(Guid userId, CancellationToken ct = default)
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
