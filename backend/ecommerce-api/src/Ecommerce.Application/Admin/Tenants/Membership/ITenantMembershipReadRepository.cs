namespace Ecommerce.Application.Admin.Tenants.Membership;

public interface ITenantMembershipReadRepository
{
    Task<List<MyTenantDto>> GetTenantsForUserAsync(Guid userId, CancellationToken ct = default);
}
