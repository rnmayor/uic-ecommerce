using Ecommerce.Application.Admin.Stores.Brands.GetAll;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Repositories.Stores.Brands.GetAll;

public sealed class GetAllStoreBrandsRepository : IGetAllStoreBrandsRepository
{
    private readonly EcommerceDbContext _context;
    public GetAllStoreBrandsRepository(EcommerceDbContext context)
    {
        _context = context;

    }
    public async Task<IReadOnlyList<StoreBrandDto>> GetAllAsync(CancellationToken ct = default)
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
