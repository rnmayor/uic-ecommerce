namespace Ecommerce.Application.Admin.Stores.Brands.GetAll;

public interface IGetAllStoreBrandsRepository
{
    Task<IReadOnlyList<StoreBrandDto>> GetAllAsync(CancellationToken ct = default);
}

public sealed class GetAllStoreBrandsService : IGetAllStoreBrandsService
{
    private readonly IGetAllStoreBrandsRepository _repository;

    public GetAllStoreBrandsService(IGetAllStoreBrandsRepository repository)
    {
        _repository = repository;
    }

    public async Task<StoreBrandsResponse> HandleAsync(CancellationToken ct = default)
    {
        return new StoreBrandsResponse
        {
            Brands = await _repository.GetAllAsync(ct)
        };
    }
}
