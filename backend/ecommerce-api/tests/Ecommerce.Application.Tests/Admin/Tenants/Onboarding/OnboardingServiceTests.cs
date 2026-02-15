using Ecommerce.Application.Admin.Tenants.Onboarding;
using Ecommerce.Application.Common.Persistence;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Tenants;

namespace Ecommerce.Application.Tests.Admin.Tenants.Onboarding;

public sealed class OnboardingServiceTests
{
    private readonly Mock<IOnboardingRepository> _onboardingRepoMock;
    private readonly Mock<ITenantRepository> _tenantRepoMock;
    private readonly OnboardingService _service;

    public OnboardingServiceTests()
    {
        _onboardingRepoMock = new Mock<IOnboardingRepository>();
        _tenantRepoMock = new Mock<ITenantRepository>();
        _service = new OnboardingService(_onboardingRepoMock.Object, _tenantRepoMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_CreatesTenantAndOwner_WhenUserDoesNotOwnTenant()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new OnboardingRequest
        {
            TenantName = "My Tenant",
        };

        _onboardingRepoMock
            .Setup(r => r.UserAlreadyHasTenantAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var response = await _service.ExecuteAsync(userId, request, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, response.TenantId);

        _tenantRepoMock.Verify(r => r.CreateAsync(
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
    public async Task ExecuteAsync_Throws_WhenUserAlreadyOwnsTenant()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new OnboardingRequest
        {
            TenantName = "My Tenant",
        };

        _onboardingRepoMock
            .Setup(r => r.UserAlreadyHasTenantAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            _service.ExecuteAsync(userId, request, CancellationToken.None)
        );

        // Assert
        Assert.Contains("User already owns a tenant", ex.Message);

        _tenantRepoMock.Verify(r => r.CreateAsync(
            It.IsAny<Tenant>(),
            It.IsAny<TenantUser>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_AlwaysCheckOwnershipBeforeCreating()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new OnboardingRequest
        {
            TenantName = "My Tenant",
        };

        _onboardingRepoMock
            .Setup(r => r.UserAlreadyHasTenantAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        await _service.ExecuteAsync(userId, request, CancellationToken.None);

        // Assert
        _onboardingRepoMock.Verify(r => r.UserAlreadyHasTenantAsync(
            userId, It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_PassesCancellationToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new OnboardingRequest
        {
            TenantName = "My Tenant",
        };
        using var cts = new CancellationTokenSource();

        _onboardingRepoMock
            .Setup(r => r.UserAlreadyHasTenantAsync(userId, cts.Token))
            .ReturnsAsync(false);

        // Act
        var response = await _service.ExecuteAsync(userId, request, cts.Token);

        _tenantRepoMock.Verify(r => r.CreateAsync(
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
