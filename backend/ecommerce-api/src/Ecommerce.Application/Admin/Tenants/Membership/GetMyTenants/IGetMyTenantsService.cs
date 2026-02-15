namespace Ecommerce.Application.Admin.Tenants.Membership.GetMyTenants;

public interface IGetMyTenantsService
{
    Task<MyTenantsResponse> HandleAsync(Guid userId, CancellationToken ct = default);
}
