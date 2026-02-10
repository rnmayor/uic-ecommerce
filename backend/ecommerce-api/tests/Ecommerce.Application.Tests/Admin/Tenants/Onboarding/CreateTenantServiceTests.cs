using Ecommerce.Application.Admin.Tenants.Onboarding;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Tenants;

namespace Ecommerce.Application.Tests.Admin.Tenants.Onboarding;

public sealed class CreateTenantServiceTests
{
    private readonly Mock<ITenantOnboardingRepository> _repositoryMock;
    private readonly CreateTenantService _service;

    public CreateTenantServiceTests()
    {
        _repositoryMock = new Mock<ITenantOnboardingRepository>();
        _service = new CreateTenantService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_CreatesTenantAndOwner_WhenUserDoesNotOwnTenant()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new CreateTenantRequest
        {
            TenantName = "My Tenant",
        };

        _repositoryMock
            .Setup(r => r.UserOwnsTenantAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var response = await _service.CreateAsync(userId, request, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, response.TenantId);

        _repositoryMock.Verify(r => r.CreateTenantAsync(
            It.Is<Tenant>(t =>
                t.Name == request.TenantName &&
                t.OwnerUserId == userId
            ),
            It.Is<TenantUser>(tu =>
                tu.TenantId == response.TenantId &&
                tu.UserId == userId &&
                tu.Role == TenantRoles.Owner
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenUserAlreadyOwnsTenant()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new CreateTenantRequest
        {
            TenantName = "My Tenant",
        };

        _repositoryMock
            .Setup(r => r.UserOwnsTenantAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            _service.CreateAsync(userId, request, CancellationToken.None)
        );

        // Assert
        Assert.Contains("User already owns a tenant", ex.Message);

        _repositoryMock.Verify(r => r.CreateTenantAsync(
            It.IsAny<Tenant>(),
            It.IsAny<TenantUser>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_AlwaysCheckOwnershipBeforeCreating()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new CreateTenantRequest
        {
            TenantName = "My Tenant",
        };

        _repositoryMock
            .Setup(r => r.UserOwnsTenantAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        await _service.CreateAsync(userId, request, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.UserOwnsTenantAsync(
            userId, It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task CreateTenantAsync_PassesCancellationToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new CreateTenantRequest
        {
            TenantName = "My Tenant",
        };
        using var cts = new CancellationTokenSource();

        _repositoryMock
            .Setup(r => r.UserOwnsTenantAsync(userId, cts.Token))
            .ReturnsAsync(false);

        // Act
        var response = await _service.CreateAsync(userId, request, cts.Token);

        _repositoryMock.Verify(r => r.CreateTenantAsync(
            It.Is<Tenant>(t =>
                t.Name == request.TenantName &&
                t.OwnerUserId == userId
            ),
            It.Is<TenantUser>(tu =>
                tu.TenantId == response.TenantId &&
                tu.UserId == userId &&
                tu.Role == TenantRoles.Owner
            ),
            cts.Token
        ), Times.Once);
    }
}
