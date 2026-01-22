using Ecommerce.Application.Common.Authorization;
using Ecommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Authorization;

public sealed class TenantAuthorizationService : ITenantAuthorizationService
{
    private readonly EcommerceDbContext _context;
    public TenantAuthorizationService(EcommerceDbContext context)
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
