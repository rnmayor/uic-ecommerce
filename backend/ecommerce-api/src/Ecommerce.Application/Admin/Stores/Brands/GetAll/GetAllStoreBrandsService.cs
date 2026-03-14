using Ecommerce.Domain.Common;

namespace Ecommerce.Application.Admin.Stores.Brands.GetAll;

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

        var result = await _repository.GetAllAsync(sanitizedQuery, ct);
        if (result.IsFailure)
        {
            return result.Error;
        }

        return new StoreBrandsResponse
        {
            Brands = result.Value.Items,
            TotalCount = result.Value.TotalCount
        };
    }
}
