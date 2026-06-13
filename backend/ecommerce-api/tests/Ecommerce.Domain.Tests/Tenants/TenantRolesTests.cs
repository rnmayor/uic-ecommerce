using Ecommerce.Domain.Tenants;

namespace Ecommerce.Domain.Tests.Tenants;

public sealed class TenantRolesTests
{
    [Fact]
    public void All_ShouldContainAllDefinedRoles()
    {
        var roles = TenantRoles.All;

        Assert.Contains(TenantRoles.Owner, roles);
        Assert.Contains(TenantRoles.Admin, roles);
        Assert.Contains(TenantRoles.Manager, roles);
        Assert.Contains(TenantRoles.Staff, roles);
        Assert.Contains(TenantRoles.Customer, roles);
    }
}
