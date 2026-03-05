namespace Ecommerce.Application.Admin.Stores.Brands.Create;

public record CreateStoreBrandRequest
{
    public string StoreBrandName { get; init; } = default!;
}

public record CreateStoreBrandResponse
{
    public Guid StoreBrandId { get; init; }
}
