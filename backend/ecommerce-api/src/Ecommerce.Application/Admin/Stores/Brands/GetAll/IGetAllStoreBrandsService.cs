namespace Ecommerce.Application.Admin.Stores.Brands.GetAll;

public interface IGetAllStoreBrandsService
{
    Task<StoreBrandsResponse> HandleAsync(GetAllBrandsQuery query, CancellationToken ct = default);
}
