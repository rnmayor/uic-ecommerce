using Ecommerce.Application.Admin.Tenants.Onboarding;
using Ecommerce.Domain.Tenants;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Onboarding;

public sealed class TenantOnboardingRepository : ITenantOnboardingRepository
{
    private readonly EcommerceDbContext _context;
    public TenantOnboardingRepository(EcommerceDbContext context)
    {
        _context = context;
    }

    public async Task CreateTenantAsync(Tenant tenant, TenantUser owner, Store store, CancellationToken ct = default)
    {
        _context.Tenants.Add(tenant);
        _context.TenantUsers.Add(owner);
        _context.Stores.Add(store);

        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> UserOwnsTenantAsync(Guid userId, CancellationToken ct = default)
    {
        return await _context.Tenants.AnyAsync(t => t.OwnerUserId == userId.ToString(), ct);
    }
}
