using Ecommerce.Application.Common.Persistence;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Stores;

namespace Ecommerce.Application.Admin.Stores.Brands.Create;

public sealed class CreateStoreBrandService : ICreateStoreBrandService
{
    private readonly IStoreBrandRepository _repository;
    public CreateStoreBrandService(IStoreBrandRepository repository)
    {
        _repository = repository;
    }
    public async Task<CreateStoreBrandResponse> ExecuteAsync(CreateStoreBrandRequest request, CancellationToken ct = default)
    {
        var normalizedName = StoreBrand.Normalize(request.StoreBrandName);

        if (await _repository.StoreBrandExistAsync(normalizedName, ct))
        {
            throw new DomainException($"Store brand {request.StoreBrandName} already exists.");
        }

        var storeBrand = new StoreBrand(request.StoreBrandName);

        await _repository.CreateAsync(storeBrand, ct);

        return new CreateStoreBrandResponse
        {
            StoreBrandId = storeBrand.Id
        };
    }
}
