using Ecommerce.Domain.Common;
using Ecommerce.Domain.Tenants;

namespace Ecommerce.Domain.Tests.Tenants;

public sealed class TenantTests
{
    [Fact]
    public void CreatesTenant_WhenValid()
    {
        var name = "My Tenant";
        var ownerUserId = Guid.NewGuid();

        var tenant = new Tenant(name, ownerUserId);

        Assert.NotEqual(Guid.Empty, tenant.Id);
        Assert.Equal(name, tenant.Name);
        Assert.Equal(ownerUserId, tenant.OwnerUserId);
        Assert.Equal(tenant.CreatedAt, tenant.UpdatedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Throws_WhenTenantIsNullOrWhiteSpace(string name)
    {
        var ownerUserId = Guid.NewGuid();
        var ex = Assert.Throws<DomainException>(() =>
            new Tenant(name, ownerUserId));

        Assert.Contains("Tenant name is required", ex.Message);
    }

    [Fact]
    public void Throws_WhenOwnerUserIdIsEmpty()
    {
        var name = "My Tenant";
        var ex = Assert.Throws<DomainException>(() =>
            new Tenant(name, Guid.Empty));

        Assert.Contains("OwnerUserId is required", ex.Message);
    }

    [Fact]
    public void TrimsName_OnCreation()
    {
        var name = "   My Tenant   ";
        var ownerUserId = Guid.NewGuid();
        var tenant = new Tenant(name, ownerUserId);

        Assert.Equal("My Tenant", tenant.Name);
    }

    [Fact]
    public void SetsCreatedAtToUtcNow()
    {
        var name = "My Tenant";
        var ownerUserId = Guid.NewGuid();

        var before = DateTime.UtcNow;
        var tenant = new Tenant(name, ownerUserId);
        var after = DateTime.UtcNow;

        Assert.InRange(tenant.CreatedAt, before, after);
    }
}
