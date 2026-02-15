using Ecommerce.Application.Admin.Tenants.Onboarding;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Repositories.Tenants.Onboarding;

public sealed class OnboardingRepository : IOnboardingRepository
{
    private readonly EcommerceDbContext _context;
    public OnboardingRepository(EcommerceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> UserAlreadyHasTenantAsync(Guid userId, CancellationToken ct = default)
    {
        return await _context.Tenants
            .IgnoreQueryFilters()
            .AnyAsync(t => t.OwnerUserId == userId, ct);
    }
}
