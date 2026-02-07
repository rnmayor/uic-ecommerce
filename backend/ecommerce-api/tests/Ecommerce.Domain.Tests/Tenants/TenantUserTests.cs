using Ecommerce.Domain.Common;
using Ecommerce.Domain.Tenants;

namespace Ecommerce.Domain.Tests.Tenants;

public sealed class TenantUserTests
{
    [Fact]
    public void CreatesTenantUser_WhenValid()
    {
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var tenantUser = new TenantUser(tenantId, userId, TenantRoles.Owner);

        Assert.NotEqual(Guid.Empty, tenantUser.Id);
        Assert.Equal(tenantId, tenantUser.TenantId);
        Assert.Equal(userId, tenantUser.UserId);
        Assert.Equal(TenantRoles.Owner, tenantUser.Role);
    }

    [Fact]
    public void Throws_WhenTenantIdIsEmpty()
    {
        var userId = Guid.NewGuid();

        var ex = Assert.Throws<DomainException>(() =>
            new TenantUser(Guid.Empty, userId, TenantRoles.Owner));

        Assert.Contains("TenantId", ex.Message);
    }

    [Fact]
    public void Throws_WhenUserIdIsEmpty()
    {
        var tenantId = Guid.NewGuid();

        var ex = Assert.Throws<DomainException>(() =>
            new TenantUser(tenantId, Guid.Empty, TenantRoles.Owner));

        Assert.Contains("UserId", ex.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Throws_WhenRolesIsNullOrWhitespace(string role)
    {
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var ex = Assert.Throws<DomainException>(() =>
            new TenantUser(tenantId, userId, role));

        Assert.Contains("Role is required", ex.Message);
    }

    [Fact]
    public void Throws_WhenRolesIsInvalid()
    {
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var ex = Assert.Throws<DomainException>(() =>
            new TenantUser(tenantId, userId, "SuperAdmin"));

        Assert.Contains("Invalid tenant role", ex.Message);
    }

    [Fact]
    public void SetsCreatedAtToUtcNow()
    {
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var before = DateTime.UtcNow;
        var tenantUser = new TenantUser(tenantId, userId, TenantRoles.Owner);
        var after = DateTime.UtcNow;

        Assert.InRange(tenantUser.CreatedAt, before, after);
    }
}
