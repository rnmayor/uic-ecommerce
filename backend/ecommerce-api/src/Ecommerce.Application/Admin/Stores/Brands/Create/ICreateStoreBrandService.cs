namespace Ecommerce.Application.Admin.Stores.Brands.Create;

public interface ICreateStoreBrandService
{
    Task<CreateStoreBrandResponse> ExecuteAsync(CreateStoreBrandRequest request, CancellationToken ct = default);
}
