using Ecommerce.Domain.Common;
using Ecommerce.Domain.Stores;

namespace Ecommerce.Domain.Tests.Stores;

public sealed class StoreInstanceTests
{
    [Fact]
    public void CreatesStoreInstance_WhenValid()
    {
        var tenantId = Guid.NewGuid();
        var storeBrandId = Guid.NewGuid();
        var displayName = "My Store";

        var storeInstance = new StoreInstance(tenantId, storeBrandId, displayName);

        Assert.NotEqual(Guid.Empty, storeInstance.Id);
        Assert.Equal(tenantId, storeInstance.TenantId);
        Assert.Equal(storeBrandId, storeInstance.StoreBrandId);
        Assert.Equal(displayName, storeInstance.DisplayName);
        Assert.Equal(storeInstance.CreatedAt, storeInstance.UpdatedAt);
    }

    [Fact]
    public void Throws_WhenTenantIdIsEmpty()
    {
        var tenantId = Guid.Empty;
        var storeBrandId = Guid.NewGuid();
        var displayName = "My Store";

        var ex = Assert.Throws<DomainException>(() =>
          new StoreInstance(tenantId, storeBrandId, displayName));

        Assert.Contains("TenantId is required", ex.Message);
    }

    [Fact]
    public void Throws_WhenStoreBrandIdIsEmpty()
    {
        var tenantId = Guid.NewGuid();
        var storeBrandId = Guid.Empty;
        var displayName = "My Store";

        var ex = Assert.Throws<DomainException>(() =>
          new StoreInstance(tenantId, storeBrandId, displayName));

        Assert.Contains("StoreBrandId is required", ex.Message);

    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Throws_WhenDisplayNameIsNullOrWhitespace(string displayName)
    {
        var tenantId = Guid.NewGuid();
        var storeBrandId = Guid.NewGuid();

        var ex = Assert.Throws<DomainException>(() =>
            new StoreInstance(tenantId, storeBrandId, displayName));

        Assert.Contains("Store display name is required", ex.Message);
    }

    [Fact]
    public void TrimsDisplayName_OnCreation()
    {
        var tenantId = Guid.NewGuid();
        var storeBrandId = Guid.NewGuid();
        var displayName = "  My Store  ";

        var storeInstance = new StoreInstance(tenantId, storeBrandId, displayName);

        Assert.Equal("My Store", storeInstance.DisplayName);
    }

    [Fact]
    public void SetsCreatedAtToUtcNow()
    {
        var tenantId = Guid.NewGuid();
        var storeBrandId = Guid.NewGuid();
        var displayName = "My Store";

        var before = DateTime.UtcNow;
        var storeInstance = new StoreInstance(tenantId, storeBrandId, displayName);
        var after = DateTime.UtcNow;

        Assert.InRange(storeInstance.CreatedAt, before, after);
    }
}
