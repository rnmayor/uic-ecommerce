namespace Ecommerce.Application.Admin.Stores.Brands;

public sealed class StoreBrandDto
{
    public Guid BrandId { get; init; }
    public string Name { get; init; } = default!;
}
