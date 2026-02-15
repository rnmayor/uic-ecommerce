using Ecommerce.Application.Admin.Tenants.Membership.GetMyTenants;
using Ecommerce.Domain.Tenants;

namespace Ecommerce.Application.Tests.Admin.Tenants.Membership.GetMyTenants;

public sealed class GetMyTenantsServiceTests
{
    private readonly Mock<IGetMyTenantsRepository> _repositoryMock;
    private readonly GetMyTenantsService _service;

    public GetMyTenantsServiceTests()
    {
        _repositoryMock = new Mock<IGetMyTenantsRepository>();
        _service = new GetMyTenantsService(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ReturnsTenantList_WhenUserHasMemberships()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tenants = new List<MyTenantDto>
        {
            new()
            {
                TenantId = Guid.NewGuid(),
                Name = "Tenant A",
                IsOwner = true,
                Role = TenantRoles.Owner
            },
            new()
            {
                TenantId = Guid.NewGuid(),
                Name = "Tenant B",
                IsOwner = false,
                Role = TenantRoles.Admin
            }
        };

        _repositoryMock
            .Setup(r => r.GetTenantsForUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenants);

        // Act
        var response = await _service.HandleAsync(userId, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(2, response.Tenants.Count);
        Assert.True(response.HasTenant);
        Assert.Same(tenants, response.Tenants);

        _repositoryMock.Verify(r => r.GetTenantsForUserAsync(
            userId, It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ReturnsEmptyList_WhenUserHasNoTenants()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _repositoryMock
            .Setup(r => r.GetTenantsForUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MyTenantDto>());

        // Act
        var response = await _service.HandleAsync(userId, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Empty(response.Tenants);
        Assert.False(response.HasTenant);

        _repositoryMock.Verify(r => r.GetTenantsForUserAsync(
            userId, It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Theory]
    [InlineData(TenantRoles.Admin)]
    [InlineData(TenantRoles.Manager)]
    [InlineData(TenantRoles.Staff)]
    [InlineData(TenantRoles.Customer)]
    public async Task HandleAsync_IsOwnerIsFalse_ForNonOwnerRoles(string role)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tenants = new List<MyTenantDto>
        {
            new()
            {
                TenantId = Guid.NewGuid(),
                Name = "Tenant",
                Role = role,
                IsOwner = false
            }
        };

        _repositoryMock
            .Setup(r => r.GetTenantsForUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenants);

        // Act
        var response = await _service.HandleAsync(userId, CancellationToken.None);

        // Assert
        Assert.False(response.Tenants.Single().IsOwner);

        _repositoryMock.Verify(r => r.GetTenantsForUserAsync(
            userId, It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_PassesCancellationToken()
    {
        var userId = Guid.NewGuid();
        using var cts = new CancellationTokenSource();

        _repositoryMock
            .Setup(r => r.GetTenantsForUserAsync(userId, cts.Token))
            .ReturnsAsync(new List<MyTenantDto>());

        await _service.HandleAsync(userId, cts.Token);

        _repositoryMock.Verify(r => r.GetTenantsForUserAsync(userId, cts.Token), Times.Once);
    }
}
