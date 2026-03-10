namespace Ecommerce.Application.Admin.Stores.Brands.GetAll;

public interface IGetAllStoreBrandsRepository
{
    Task<(IReadOnlyList<StoreBrandDTO> Items, int TotalCount)> GetAllAsync(GetAllBrandsQuery query, CancellationToken ct = default);
}

public sealed class GetAllStoreBrandsService : IGetAllStoreBrandsService
{
    private readonly IGetAllStoreBrandsRepository _repository;
    private const int MaxLimit = 100;
    private const int DefaultLimit = 30;

    public GetAllStoreBrandsService(IGetAllStoreBrandsRepository repository)
    {
        _repository = repository;
    }

    public async Task<StoreBrandsResponse> HandleAsync(GetAllBrandsQuery query, CancellationToken ct = default)
    {
        var sanitizedQuery = query with
        {
            Skip = Math.Max(0, query.Skip),
            Limit = query.Limit <= 0 ? DefaultLimit : Math.Min(query.Limit, MaxLimit)
        };

        var (items, total) = await _repository.GetAllAsync(sanitizedQuery, ct);

        return new StoreBrandsResponse
        {
            Brands = items,
            TotalCount = total
        };
    }
}
