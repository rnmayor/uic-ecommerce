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

    public async Task CreateTenantAsync(Tenant tenant, TenantUser owner, CancellationToken ct = default)
    {
        using var tx = await _context.Database.BeginTransactionAsync(ct);

        _context.Tenants.Add(tenant);
        _context.TenantUsers.Add(owner);

        await _context.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
    }

    public async Task<bool> UserOwnsTenantAsync(Guid userId, CancellationToken ct = default)
    {
        return await _context.Tenants
            .IgnoreQueryFilters()
            .AnyAsync(t => t.OwnerUserId == userId, ct);
    }
}
