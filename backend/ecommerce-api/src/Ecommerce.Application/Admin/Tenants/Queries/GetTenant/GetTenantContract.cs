using Ecommerce.Domain.Common;

namespace Ecommerce.Application.Admin.Tenants.Queries.GetTenant
{
    public sealed record GetTenantDTO
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = default!;
        public string Slug { get; init; } = default!;
    }

    public sealed record GetTenantStoreDTO
    {
        public Guid Id { get; init; }
        public string DisplayName { get; init; } = default!;
        public string StoreBrandName { get; init; } = default!;
    }

    public sealed record GetTenantResponse
    {
        public GetTenantDTO Tenant { get; init; } = new GetTenantDTO();
        public IReadOnlyCollection<GetTenantStoreDTO> Stores { get; init; } = new List<GetTenantStoreDTO>();
    }

    public interface IGetTenantService
    {
        Task<Result<GetTenantResponse>> HandleAsync(string slug, CancellationToken ct = default);
    }

    public interface IGetTenantRepository
    {
        Task<GetTenantResponse?> GetTenantAsync(Guid tenantId, CancellationToken ct = default);
    }
}
