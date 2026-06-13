using Ecommerce.Domain.Stores;

namespace Ecommerce.Application.Common.Persistence;

public interface IStoreBrandRepository
{
    Task<bool> StoreBrandExistAsync(string normalizedName, CancellationToken ct = default);
    Task CreateAsync(StoreBrand storeBrand, CancellationToken ct = default);
}
