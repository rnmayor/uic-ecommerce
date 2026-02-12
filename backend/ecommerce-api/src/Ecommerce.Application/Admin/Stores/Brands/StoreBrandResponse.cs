using Ecommerce.Application.Admin.Stores.Brands;

namespace Ecommerce.Application.Stores.Brands;

public sealed class StoreBrandResponse
{
    public List<StoreBrandDto> Brands { get; init; } = [];
}
