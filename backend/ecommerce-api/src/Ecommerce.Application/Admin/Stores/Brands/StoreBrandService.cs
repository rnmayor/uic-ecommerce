using Ecommerce.Application.Stores.Brands;

namespace Ecommerce.Application.Admin.Stores.Brands;

public sealed class StoreBrandService : IStoreBrandService
{
    private readonly IStoreBrandReadRepository _readRepository;

    public StoreBrandService(IStoreBrandReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<StoreBrandResponse> GetAllAsync(CancellationToken ct = default)
    {
        return new StoreBrandResponse
        {
            Brands = await _readRepository.GetAllAsync(ct)
        };
    }
}
