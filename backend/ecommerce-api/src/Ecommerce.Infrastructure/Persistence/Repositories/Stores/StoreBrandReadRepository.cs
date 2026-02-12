using Ecommerce.Application.Admin.Stores.Brands;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Repositories.Stores;

public sealed class StoreBrandReadRepository : IStoreBrandReadRepository
{
    private readonly EcommerceDbContext _context;
    public StoreBrandReadRepository(EcommerceDbContext context)
    {
        _context = context;

    }
    public async Task<List<StoreBrandDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.StoreBrands
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Select(b => new StoreBrandDto
            {
                BrandId = b.Id,
                Name = b.Name
            })
            .ToListAsync(ct);
    }
}
