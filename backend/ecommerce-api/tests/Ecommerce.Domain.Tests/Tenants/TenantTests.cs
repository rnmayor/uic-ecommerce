using Ecommerce.Domain.Common;
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

            var tenant = Tenant.Create(name, ownerUserId);

            Assert.True(tenant.IsSuccess);
            Assert.NotEqual(Guid.Empty, tenant.Value.Id);
            Assert.Equal(name, tenant.Value.Name);
            Assert.Equal(IdentityNormalizer.Normalize(name), tenant.Value.NormalizedName);
            Assert.Equal(SlugGenerator.Generate(name), tenant.Value.Slug);
            Assert.Equal(ownerUserId, tenant.Value.OwnerUserId);
            Assert.Equal(tenant.Value.CreatedAt, tenant.Value.UpdatedAt);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void ReturnsFailure_WhenTenantIsNullOrWhiteSpace(string name)
        {
            var ownerUserId = Guid.NewGuid();
            var tenant = Tenant.Create(name, ownerUserId);

            Assert.True(tenant.IsFailure);
            Assert.Equal(TenantErrors.NameRequired, tenant.Error);
        }

        [Fact]
        public void ReturnsFailure_WhenOwnerUserIdIsEmpty()
        {
            var name = "My Tenant";
            var tenant = Tenant.Create(name, Guid.Empty);

            Assert.True(tenant.IsFailure);
            Assert.Equal(TenantErrors.OwnerRequired, tenant.Error);
        }

        [Fact]
        public void TrimsName_OnCreation()
        {
            var name = "   My Tenant   ";
            var ownerUserId = Guid.NewGuid();
            var tenant = Tenant.Create(name, ownerUserId);

            Assert.True(tenant.IsSuccess);
            Assert.Equal("My Tenant", tenant.Value.Name);
        }

        [Fact]
        public void SetsCreatedAtToUtcNow()
        {
            var name = "My Tenant";
            var ownerUserId = Guid.NewGuid();

            var before = DateTime.UtcNow;
            var tenant = Tenant.Create(name, ownerUserId);
            var after = DateTime.UtcNow;

            Assert.True(tenant.IsSuccess);
            Assert.InRange(tenant.Value.CreatedAt, before, after);
        }
    }
}
