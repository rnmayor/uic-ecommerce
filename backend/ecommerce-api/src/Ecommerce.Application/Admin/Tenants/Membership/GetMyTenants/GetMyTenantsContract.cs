using Ecommerce.Domain.Common;

namespace Ecommerce.Application.Admin.Tenants.Membership.GetMyTenants
{
    public sealed record MyTenantDTO
    {
        public Guid TenantId { get; init; }
        public string Name { get; init; } = default!;
        public bool IsOwner { get; init; }
        public string Role { get; init; } = default!;
    }

    public sealed record MyTenantsResponse
    {
        public IReadOnlyList<MyTenantDTO> Tenants { get; init; } = new List<MyTenantDTO>();
        public bool HasTenant => Tenants.Count > 0;
    }

    public interface IGetMyTenantsService
    {
        Task<Result<MyTenantsResponse>> HandleAsync(Guid userId, CancellationToken ct = default);
    }

    public interface IGetMyTenantsRepository
    {
        Task<Result<IReadOnlyList<MyTenantDTO>>> GetTenantsForUserAsync(Guid userId, CancellationToken ct = default);
    }
}