namespace Ecommerce.Application.Admin.Stores.Brands.GetAll;

public sealed record StoreBrandDTO
{
    public Guid BrandId { get; init; }
    public string Name { get; init; } = default!;
}

public sealed record StoreBrandsResponse
{
    public IReadOnlyList<StoreBrandDTO> Brands { get; init; } = new List<StoreBrandDTO>();
    public int TotalCount { get; init; }
}

public sealed record GetAllBrandsQuery
{
    public int Skip { get; init; }
    public int Limit { get; init; }
    public string? Search { get; init; }
}
