using Ecommerce.Application.Admin.Tenants.Features.Onboarding;
using Ecommerce.Domain.Tenants;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Repositories.Tenants.Features.Onboarding
{
    public sealed class OnboardingRepository : IOnboardingRepository
    {
        private readonly EcommerceDbContext _context;
        public OnboardingRepository(EcommerceDbContext context)
        {
            _context = context;
        }

        public async Task CreateTenantWithOwnerAsync(Tenant tenant, TenantUser owner, CancellationToken ct = default)
        {
            using var tx = await _context.Database.BeginTransactionAsync(ct);

            _context.Tenants.Add(tenant);
            _context.TenantUsers.Add(owner);

            await _context.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }

        public async Task<bool> TenantExistAsync(string normalizedName, CancellationToken ct = default)
        {
            return await _context.Tenants.AnyAsync(x => x.NormalizedName == normalizedName, ct);
        }

        public async Task<bool> UserAlreadyHasTenantAsync(Guid userId, CancellationToken ct = default)
        {
            return await _context.Tenants
                .IgnoreQueryFilters()
                .AnyAsync(t => t.OwnerUserId == userId, ct);
        }
    }
}
