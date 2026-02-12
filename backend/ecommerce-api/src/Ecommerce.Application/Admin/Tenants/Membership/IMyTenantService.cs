namespace Ecommerce.Application.Admin.Tenants.Membership;

public interface IMyTenantService
{
    Task<MyTenantResponse> GetMyTenantsAsync(Guid userId, CancellationToken ct = default);
}
