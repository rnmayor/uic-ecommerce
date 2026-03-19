using Ecommerce.Domain.Common;

namespace Ecommerce.Application.Admin.Stores.Brands.Queries.GetAllStoreBrands;

public sealed class GetAllStoreBrandsService : IGetAllStoreBrandsService
{
    private readonly IGetAllStoreBrandsRepository _repository;
    private const int MaxLimit = 100;
    private const int DefaultLimit = 30;

    public GetAllStoreBrandsService(IGetAllStoreBrandsRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<StoreBrandsResponse>> HandleAsync(GetAllBrandsQuery query, CancellationToken ct = default)
    {
        var sanitizedQuery = query with
        {
            Skip = Math.Max(0, query.Skip),
            Limit = query.Limit <= 0 ? DefaultLimit : Math.Min(query.Limit, MaxLimit),
            Search = string.IsNullOrWhiteSpace(query.Search) ? null : query.Search.Trim()
        };

        var (items, total) = await _repository.GetAllAsync(sanitizedQuery, ct);

        return new StoreBrandsResponse
        {
            Brands = items,
            Total = total,
            Skip = sanitizedQuery.Skip,
            Limit = sanitizedQuery.Limit
        };
    }
}
