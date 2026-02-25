namespace Ecommerce.Application.Admin.Stores.Brands.Create;

public interface ICreateStorBrandService
{
    Task<CreateStoreBrandResponse> ExecuteAsync(CreateStoreBrandRequest request, CancellationToken ct = default);
}
