namespace Ecommerce.Application.Admin.Tenants.Membership.GetMyTenants;

public sealed class MyTenantsResponse
{
    public bool HasTenant => Tenants.Count > 0;
    public IReadOnlyList<MyTenantDto> Tenants { get; init; } = [];
}
