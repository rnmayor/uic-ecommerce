namespace Ecommerce.Application.Admin.Tenants.Membership;

public sealed class MyTenantResponse
{
    public bool HasTenant => Tenants.Count > 0;
    public List<MyTenantDto> Tenants { get; init; } = [];
}
