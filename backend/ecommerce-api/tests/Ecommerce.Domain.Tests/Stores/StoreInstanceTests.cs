using Ecommerce.Domain.Stores;

namespace Ecommerce.Domain.Tests.Stores
{
    public sealed class StoreInstanceTests
    {
        [Fact]
        public void CreatesStoreInstance_WhenValid()
        {
            var tenantId = Guid.NewGuid();
            var storeBrandId = Guid.NewGuid();
            var displayName = "My Store";

            var storeInstance = StoreInstance.Create(tenantId, storeBrandId, displayName);

            Assert.True(storeInstance.IsSuccess);
            Assert.NotEqual(Guid.Empty, storeInstance.Value.Id);
            Assert.Equal(tenantId, storeInstance.Value.TenantId);
            Assert.Equal(storeBrandId, storeInstance.Value.StoreBrandId);
            Assert.Equal(displayName, storeInstance.Value.DisplayName);
            Assert.Equal(storeInstance.Value.CreatedAt, storeInstance.Value.UpdatedAt);
        }

        [Fact]
        public void ReturnsFailure_WhenTenantIdIsEmpty()
        {
            var tenantId = Guid.Empty;
            var storeBrandId = Guid.NewGuid();
            var displayName = "My Store";

            var storeInstance = StoreInstance.Create(tenantId, storeBrandId, displayName);

            Assert.True(storeInstance.IsFailure);
            Assert.Equal(StoreInstanceErrors.TenantRequired, storeInstance.Error);
        }

        [Fact]
        public void ReturnsFailure_WhenStoreBrandIdIsEmpty()
        {
            var tenantId = Guid.NewGuid();
            var storeBrandId = Guid.Empty;
            var displayName = "My Store";

            var storeInstance = StoreInstance.Create(tenantId, storeBrandId, displayName);

            Assert.True(storeInstance.IsFailure);
            Assert.Equal(StoreInstanceErrors.StoreBrandRequired, storeInstance.Error);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void ReturnsFailure_WhenDisplayNameIsNullOrWhitespace(string displayName)
        {
            var tenantId = Guid.NewGuid();
            var storeBrandId = Guid.NewGuid();

            var storeInstance = StoreInstance.Create(tenantId, storeBrandId, displayName);

            Assert.True(storeInstance.IsFailure);
            Assert.Equal(StoreInstanceErrors.NameRequired, storeInstance.Error);
        }

        [Fact]
        public void TrimsDisplayName_OnCreation()
        {
            var tenantId = Guid.NewGuid();
            var storeBrandId = Guid.NewGuid();
            var displayName = "  My Store  ";

            var storeInstance = StoreInstance.Create(tenantId, storeBrandId, displayName);

            Assert.True(storeInstance.IsSuccess);
            Assert.Equal("My Store", storeInstance.Value.DisplayName);
        }

        [Fact]
        public void SetsCreatedAtToUtcNow()
        {
            var tenantId = Guid.NewGuid();
            var storeBrandId = Guid.NewGuid();
            var displayName = "My Store";

            var before = DateTime.UtcNow;
            var storeInstance = StoreInstance.Create(tenantId, storeBrandId, displayName);
            var after = DateTime.UtcNow;

            Assert.True(storeInstance.IsSuccess);
            Assert.InRange(storeInstance.Value.CreatedAt, before, after);
        }
    }
}