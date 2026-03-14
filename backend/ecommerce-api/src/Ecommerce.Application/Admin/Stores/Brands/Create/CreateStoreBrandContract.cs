using Ecommerce.Domain.Common;

namespace Ecommerce.Application.Admin.Stores.Brands.Create
{
    public sealed record CreateStoreBrandRequest
    {
        public string StoreBrandName { get; init; } = default!;
    }

    public sealed record CreateStoreBrandResponse
    {
        public Guid StoreBrandId { get; init; }
    }

    public interface ICreateStoreBrandService
    {
        Task<Result<CreateStoreBrandResponse>> ExecuteAsync(CreateStoreBrandRequest request, CancellationToken ct = default);
    }
}