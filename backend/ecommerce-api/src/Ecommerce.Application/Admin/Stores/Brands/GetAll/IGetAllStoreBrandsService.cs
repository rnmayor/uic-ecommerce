namespace Ecommerce.Application.Admin.Stores.Brands.GetAll;

public interface IGetAllStoreBrandsService
{
    Task<StoreBrandsResponse> HandleAsync(CancellationToken ct = default);
}
