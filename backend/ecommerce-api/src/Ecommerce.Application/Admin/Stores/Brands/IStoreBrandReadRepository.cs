namespace Ecommerce.Application.Admin.Stores.Brands;

public interface IStoreBrandReadRepository
{
    Task<List<StoreBrandDto>> GetAllAsync(CancellationToken ct = default);
}
