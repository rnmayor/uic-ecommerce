namespace Ecommerce.Application.Admin.Stores.Brands.GetAll;

public record StoreBrandDTO
{
    public Guid BrandId { get; init; }
    public string Name { get; init; } = default!;
}

public record StoreBrandsResponse
{
    public IReadOnlyList<StoreBrandDTO> Brands { get; init; } = new List<StoreBrandDTO>();
}
