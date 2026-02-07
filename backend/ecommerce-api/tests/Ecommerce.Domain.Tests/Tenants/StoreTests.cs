using Ecommerce.Domain.Common;
using Ecommerce.Domain.Tenants;

namespace Ecommerce.Domain.Tests.Tenants;

public sealed class StoreTests
{
    [Fact]
    public void CreatesStore_WhenValid()
    {
        var tenantId = Guid.NewGuid();
        var name = "My Store";

        var store = new Store(tenantId, name);

        Assert.NotEqual(Guid.Empty, store.Id);
        Assert.Equal(tenantId, store.TenantId);
        Assert.Equal(name, store.Name);
        Assert.Equal(store.CreatedAt, store.UpdatedAt);
    }

    [Fact]
    public void Throws_WhenTenantIdIsEmpty()
    {
        var name = "My Store";
        var ex = Assert.Throws<DomainException>(() =>
            new Store(Guid.Empty, name));

        Assert.Contains("Store must belong to a tenant", ex.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Throws_WhenNameIsNullOrWhitespace(string name)
    {
        var tenantId = Guid.NewGuid();
        var ex = Assert.Throws<DomainException>(() =>
            new Store(tenantId, name));

        Assert.Contains("Store name is required", ex.Message);
    }

    [Fact]
    public void TrimsName_OnCreation()
    {
        var tenantId = Guid.NewGuid();
        var store = new Store(tenantId, "   My Store   ");

        Assert.Equal("My Store", store.Name);
    }

    [Fact]
    public void SetsCreatedAtToUtcNow()
    {
        var tenantId = Guid.NewGuid();
        var name = "My Store";

        var before = DateTime.UtcNow;
        var store = new Store(tenantId, name);
        var after = DateTime.UtcNow;

        Assert.InRange(store.CreatedAt, before, after);
    }
}
