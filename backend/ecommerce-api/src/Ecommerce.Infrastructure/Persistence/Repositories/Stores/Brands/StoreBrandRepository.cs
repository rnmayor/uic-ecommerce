using Ecommerce.Application.Common.Persistence;
using Ecommerce.Domain.Stores;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Repositories.Stores.Brands;

public sealed class StoreBrandRepository : IStoreBrandRepository
{
    private readonly EcommerceDbContext _context;
    public StoreBrandRepository(EcommerceDbContext context)
    {
        _context = context;
    }
    public async Task CreateAsync(StoreBrand storeBrand, CancellationToken ct = default)
    {
        _context.StoreBrands.Add(storeBrand);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> StoreBrandExistAsync(string normalizedName, CancellationToken ct = default)
    {
        return await _context.StoreBrands.AnyAsync(x => x.NormalizedName == normalizedName, ct);
    }
}
