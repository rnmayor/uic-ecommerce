using Ecommerce.Domain.Tenants;

namespace Ecommerce.Domain.Tests.Tenants
{
    public sealed class TenantTests
    {
        [Fact]
        public void CreatesTenant_WhenValid()
        {
            var name = "My Tenant";
            var ownerUserId = Guid.NewGuid();

            var tenant = Tenant.Created(name, ownerUserId);

            Assert.True(tenant.IsSuccess);
            Assert.NotEqual(Guid.Empty, tenant.Value.Id);
            Assert.Equal(name, tenant.Value.Name);
            Assert.Equal(ownerUserId, tenant.Value.OwnerUserId);
            Assert.Equal(tenant.Value.CreatedAt, tenant.Value.UpdatedAt);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Throws_WhenTenantIsNullOrWhiteSpace(string name)
        {
            var ownerUserId = Guid.NewGuid();
            var tenant = Tenant.Created(name, ownerUserId);

            Assert.True(tenant.IsFailure);
            Assert.Equal(TenantErrors.NameRequired, tenant.Error);
        }

        [Fact]
        public void Throws_WhenOwnerUserIdIsEmpty()
        {
            var name = "My Tenant";
            var tenant = Tenant.Created(name, Guid.Empty);

            Assert.True(tenant.IsFailure);
            Assert.Equal(TenantErrors.OwnerRequired, tenant.Error);
        }

        [Fact]
        public void TrimsName_OnCreation()
        {
            var name = "   My Tenant   ";
            var ownerUserId = Guid.NewGuid();
            var tenant = Tenant.Created(name, ownerUserId);

            Assert.True(tenant.IsSuccess);
            Assert.Equal("My Tenant", tenant.Value.Name);
        }

        [Fact]
        public void SetsCreatedAtToUtcNow()
        {
            var name = "My Tenant";
            var ownerUserId = Guid.NewGuid();

            var before = DateTime.UtcNow;
            var tenant = Tenant.Created(name, ownerUserId);
            var after = DateTime.UtcNow;

            Assert.True(tenant.IsSuccess);
            Assert.InRange(tenant.Value.CreatedAt, before, after);
        }
    }
}
