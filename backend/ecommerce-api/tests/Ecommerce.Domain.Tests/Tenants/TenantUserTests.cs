using Ecommerce.Domain.Tenants;

namespace Ecommerce.Domain.Tests.Tenants
{
    public sealed class TenantUserTests
    {
        [Fact]
        public void CreatesTenantUser_WhenValid()
        {
            var tenantId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var tenantUser = TenantUser.Create(tenantId, userId, TenantRoles.Owner);

            Assert.True(tenantUser.IsSuccess);
            Assert.NotEqual(Guid.Empty, tenantUser.Value.Id);
            Assert.Equal(tenantId, tenantUser.Value.TenantId);
            Assert.Equal(userId, tenantUser.Value.UserId);
            Assert.Equal(TenantRoles.Owner, tenantUser.Value.Role);
        }

        [Fact]
        public void Throws_WhenTenantIdIsEmpty()
        {
            var userId = Guid.NewGuid();

            var tenantUser = TenantUser.Create(Guid.Empty, userId, TenantRoles.Owner);

            Assert.True(tenantUser.IsFailure);
            Assert.Equal(TenantUserErrors.TenantRequired, tenantUser.Error);
        }

        [Fact]
        public void Throws_WhenUserIdIsEmpty()
        {
            var tenantId = Guid.NewGuid();

            var tenantUser = TenantUser.Create(tenantId, Guid.Empty, TenantRoles.Owner);

            Assert.True(tenantUser.IsFailure);
            Assert.Equal(TenantUserErrors.UserRequired, tenantUser.Error);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Throws_WhenRolesIsNullOrWhitespace(string role)
        {
            var tenantId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var tenantUser = TenantUser.Create(tenantId, userId, role);

            Assert.True(tenantUser.IsFailure);
            Assert.Equal(TenantUserErrors.RoleRequired, tenantUser.Error);
        }

        [Fact]
        public void Throws_WhenRolesIsInvalid()
        {
            var tenantId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var tenantUser = TenantUser.Create(tenantId, userId, "SuperAdmin");

            Assert.True(tenantUser.IsFailure);
            Assert.Equal(TenantUserErrors.TenantRoleInvalid, tenantUser.Error);
        }

        [Fact]
        public void SetsCreatedAtToUtcNow()
        {
            var tenantId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var before = DateTime.UtcNow;
            var tenantUser = TenantUser.Create(tenantId, userId, TenantRoles.Owner);
            var after = DateTime.UtcNow;

            Assert.True(tenantUser.IsSuccess);
            Assert.InRange(tenantUser.Value.CreatedAt, before, after);
        }
    }
}