using AutoFixture.Xunit2;
using Ecommerce.Application.Admin.Tenants.Membership.GetMyTenants;
using Ecommerce.Domain.Tenants;
using Ecommerce.TestUtils.Attributes;

namespace Ecommerce.Application.Tests.Admin.Tenants.Membership.GetMyTenants;

public sealed class GetMyTenantsServiceTests
{
    [Theory, AutoMoqData]
    public async Task HandleAsync_ReturnsTenantList_WhenUserHasMemberships(
        Guid userId,
        [Frozen] Mock<IGetMyTenantsRepository> repositoryMock,
        GetMyTenantsService service)
    {
        // Arrange
        var tenants = new List<MyTenantDto>
        {
            new() { TenantId = Guid.NewGuid(), Name = "Tenant A", IsOwner = true, Role = TenantRoles.Owner },
            new() { TenantId = Guid.NewGuid(), Name = "Tenant B", IsOwner = false, Role = TenantRoles.Admin }
        };

        repositoryMock
            .Setup(r => r.GetTenantsForUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenants);

        // Act
        var response = await service.HandleAsync(userId, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(2, response.Tenants.Count);
        Assert.True(response.HasTenant);
        Assert.Same(tenants, response.Tenants);

        repositoryMock.Verify(r => r.GetTenantsForUserAsync(
            userId, It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Theory, AutoMoqData]
    public async Task HandleAsync_ReturnsEmptyList_WhenUserHasNoTenants(
        Guid userId,
        [Frozen] Mock<IGetMyTenantsRepository> repositoryMock,
        GetMyTenantsService service)
    {
        // Arrange
        repositoryMock
            .Setup(r => r.GetTenantsForUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MyTenantDto>());

        // Act
        var response = await service.HandleAsync(userId, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Empty(response.Tenants);
        Assert.False(response.HasTenant);

        repositoryMock.Verify(r => r.GetTenantsForUserAsync(
            userId, It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Theory]
    [InlineAutoMoqData(TenantRoles.Admin)]
    [InlineAutoMoqData(TenantRoles.Manager)]
    [InlineAutoMoqData(TenantRoles.Staff)]
    [InlineAutoMoqData(TenantRoles.Customer)]
    public async Task HandleAsync_IsOwnerIsFalse_ForNonOwnerRoles(
        string role,
        Guid userId,
        [Frozen] Mock<IGetMyTenantsRepository> repositoryMock,
        GetMyTenantsService service)
    {
        // Arrange
        var tenants = new List<MyTenantDto>
        {
            new() { TenantId = Guid.NewGuid(), Name = "Tenant", Role = role, IsOwner = false }
        };

        repositoryMock
            .Setup(r => r.GetTenantsForUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenants);

        // Act
        var response = await service.HandleAsync(userId, CancellationToken.None);

        // Assert
        Assert.False(response.Tenants.Single().IsOwner);

        repositoryMock.Verify(r => r.GetTenantsForUserAsync(
            userId, It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Theory, AutoMoqData]
    public async Task HandleAsync_PassesCancellationToken(
        Guid userId,
        [Frozen] Mock<IGetMyTenantsRepository> repositoryMock,
        GetMyTenantsService service)
    {
        using var cts = new CancellationTokenSource();

        repositoryMock
            .Setup(r => r.GetTenantsForUserAsync(userId, cts.Token))
            .ReturnsAsync(new List<MyTenantDto>());

        await service.HandleAsync(userId, cts.Token);

        repositoryMock.Verify(r => r.GetTenantsForUserAsync(It.IsAny<Guid>(), cts.Token), Times.Once);
    }
}
