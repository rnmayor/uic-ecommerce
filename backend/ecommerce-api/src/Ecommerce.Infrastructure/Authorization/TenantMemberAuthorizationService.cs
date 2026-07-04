using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Authorization;

public sealed class TenantMemberAuthorizationService : ITenantMemberAuthorizationService
{
    private readonly EcommerceDbContext _context;
    public TenantMemberAuthorizationService(EcommerceDbContext context)
    {
        _context = context;
    }
    public async Task<bool> IsTenantMemberAsync(Guid tenantId, Guid userId, string[] allowedRoles, CancellationToken cancellationToken = default)
    {
        return await _context.TenantUsers
            .AnyAsync(tu =>
                tu.TenantId == tenantId &&
                tu.UserId == userId &&
                allowedRoles.Contains(tu.Role),
                cancellationToken);
    }
}
