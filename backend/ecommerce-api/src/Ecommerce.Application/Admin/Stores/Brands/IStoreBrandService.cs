using Ecommerce.Application.Stores.Brands;

namespace Ecommerce.Application.Admin.Stores.Brands;

public interface IStoreBrandService
{
    Task<StoreBrandResponse> GetAllAsync(CancellationToken ct = default);
}
