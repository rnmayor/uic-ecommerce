namespace Ecommerce.Application.Admin.Stores.Brands.GetAll;

public sealed class StoreBrandsResponse
{
    public IReadOnlyList<StoreBrandDto> Brands { get; init; } = [];
}
