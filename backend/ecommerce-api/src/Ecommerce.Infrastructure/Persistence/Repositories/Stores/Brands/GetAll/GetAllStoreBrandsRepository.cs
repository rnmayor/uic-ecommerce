using Ecommerce.Application.Admin.Stores.Brands.GetAll;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Stores;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Repositories.Stores.Brands.GetAll
{
    public sealed class GetAllStoreBrandsRepository : IGetAllStoreBrandsRepository
    {
        private readonly EcommerceDbContext _context;
        public GetAllStoreBrandsRepository(EcommerceDbContext context)
        {
            _context = context;

        }
        public async Task<Result<(IReadOnlyList<StoreBrandDTO> Items, int TotalCount)>> GetAllAsync(GetAllBrandsQuery query, CancellationToken ct = default)
        {
            var baseQuery = _context.StoreBrands
                .IgnoreQueryFilters()
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var searchNormalized = StoreBrand.Normalize(query.Search);
                baseQuery = baseQuery.Where(b => b.NormalizedName.Contains(searchNormalized));
            }

            var totalCount = await baseQuery.CountAsync(ct);

            var items = await baseQuery
                .OrderBy(b => b.NormalizedName)
                .Skip(query.Skip)
                .Take(query.Limit)
                .Select(b => new StoreBrandDTO
                {
                    BrandId = b.Id,
                    Name = b.Name
                })
                .ToListAsync(ct);

            return (items, totalCount);
        }
    }
}