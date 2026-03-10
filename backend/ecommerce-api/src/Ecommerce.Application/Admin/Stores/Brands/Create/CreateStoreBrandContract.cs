namespace Ecommerce.Application.Admin.Stores.Brands.Create;

public sealed record CreateStoreBrandRequest
{
    public string StoreBrandName { get; init; } = default!;
}

public sealed record CreateStoreBrandResponse
{
    public Guid StoreBrandId { get; init; }
}
