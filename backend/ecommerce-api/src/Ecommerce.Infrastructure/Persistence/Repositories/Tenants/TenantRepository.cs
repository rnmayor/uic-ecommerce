using Ecommerce.Application.Common.Persistence;
using Ecommerce.Domain.Tenants;

namespace Ecommerce.Infrastructure.Persistence.Repositories.Tenants;

public sealed class TenantRepository : ITenantRepository
{
    private readonly EcommerceDbContext _context;
    public TenantRepository(EcommerceDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(Tenant tenant, TenantUser owner, CancellationToken ct = default)
    {
        using var tx = await _context.Database.BeginTransactionAsync(ct);

        _context.Tenants.Add(tenant);
        _context.TenantUsers.Add(owner);

        await _context.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
    }
}
