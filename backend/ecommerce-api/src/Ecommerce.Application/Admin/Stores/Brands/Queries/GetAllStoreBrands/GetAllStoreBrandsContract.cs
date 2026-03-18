using Ecommerce.Domain.Common;

namespace Ecommerce.Application.Admin.Stores.Brands.Queries.GetAllStoreBrands;

public sealed record StoreBrandDTO
{
    public Guid BrandId { get; init; }
    public string Name { get; init; } = default!;
}

public sealed record StoreBrandsResponse
{
    public IReadOnlyList<StoreBrandDTO> Brands { get; init; } = new List<StoreBrandDTO>();
    public int Total { get; init; }
    public int Limit { get; init; }
    public int Skip { get; init; }
}

public sealed record GetAllBrandsQuery
{
    public int Skip { get; init; }
    public int Limit { get; init; }
    public string? Search { get; init; }
}

public interface IGetAllStoreBrandsService
{
    Task<Result<StoreBrandsResponse>> HandleAsync(GetAllBrandsQuery query, CancellationToken ct = default);
}

public interface IGetAllStoreBrandsRepository
{
    Task<Result<(IReadOnlyList<StoreBrandDTO> Items, int TotalCount)>> GetAllAsync(GetAllBrandsQuery query, CancellationToken ct = default);
}