using Ecommerce.Application.Admin.Tenants.Membership;

namespace Ecommerce.Application.Tests.Admin.Tenants.Membership;

public sealed class MyTenantServiceTests
{
    private readonly Mock<ITenantMembershipReadRepository> _repositoryMock;
    private readonly MyTenantService _service;

    public MyTenantServiceTests()
    {
        _repositoryMock = new Mock<ITenantMembershipReadRepository>();
        _service = new MyTenantService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetMyTenantsAsync_ReturnsTenants_WhenUserHasMemberships()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tenants = new List<MyTenantDto>
        {
            new()
            {
                TenantId = Guid.NewGuid(),
                Name = "Tenant A",
                IsOwner = true
            },
            new()
            {
                TenantId = Guid.NewGuid(),
                Name = "Tenant B",
                IsOwner = false
            }
        };

        _repositoryMock
            .Setup(r => r.GetTenantsForUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenants);

        // Act
        var response = await _service.GetMyTenantsAsync(userId, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(2, response.Tenants.Count);
        Assert.True(response.HasTenant);
        Assert.Same(tenants, response.Tenants);

        _repositoryMock.Verify(r =>
            r.GetTenantsForUserAsync(userId, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task GetMyTenantsAsync_ReturnsEmptyList_WhenUserHasNoTenants()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _repositoryMock
            .Setup(r => r.GetTenantsForUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MyTenantDto>());

        // Act
        var response = await _service.GetMyTenantsAsync(userId, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Empty(response.Tenants);
        Assert.False(response.HasTenant);

        _repositoryMock.Verify(r =>
            r.GetTenantsForUserAsync(userId, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task GetMyTenantsAsync_AlwaysCallsRepository()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetTenantsForUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MyTenantDto>());

        // Act
        await _service.GetMyTenantsAsync(userId, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r =>
            r.GetTenantsForUserAsync(userId, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
