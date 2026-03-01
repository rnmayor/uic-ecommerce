using AutoFixture.Xunit2;
using Ecommerce.Application.Admin.Tenants.Onboarding;
using Ecommerce.Application.Common.Persistence;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Tenants;
using Ecommerce.TestUtils.Attributes;

namespace Ecommerce.Application.Tests.Admin.Tenants.Onboarding;

public sealed class OnboardingServiceTests
{
    [Theory, AutoMoqData]
    public async Task ExecuteAsync_CreatesTenantAndOwner_WhenUserDoesNotOwnTenant(
        Guid userId,
        [Frozen] Mock<IOnboardingRepository> onboardingRepoMock,
        [Frozen] Mock<ITenantRepository> tenantRepoMock,
        OnboardingService service)
    {
        // Arrange
        var request = new OnboardingRequest
        {
            TenantName = "My Tenant",
        };

        onboardingRepoMock
            .Setup(r => r.UserAlreadyHasTenantAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var response = await service.ExecuteAsync(userId, request, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, response.TenantId);

        tenantRepoMock.Verify(r => r.CreateAsync(
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

    [Theory, AutoMoqData]
    public async Task ExecuteAsync_Throws_WhenUserAlreadyOwnsTenant(
        Guid userId,
        [Frozen] Mock<IOnboardingRepository> onboardingRepoMock,
        [Frozen] Mock<ITenantRepository> tenantRepoMock,
        OnboardingService service)
    {
        // Arrange
        var request = new OnboardingRequest
        {
            TenantName = "My Tenant",
        };

        onboardingRepoMock
            .Setup(r => r.UserAlreadyHasTenantAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            service.ExecuteAsync(userId, request, CancellationToken.None)
        );

        // Assert
        Assert.Contains("User already owns a tenant", ex.Message);

        tenantRepoMock.Verify(r => r.CreateAsync(
            It.IsAny<Tenant>(),
            It.IsAny<TenantUser>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);
    }

    [Theory, AutoMoqData]
    public async Task ExecuteAsync_PassesCancellationToken(
        Guid userId,
        [Frozen] Mock<IOnboardingRepository> onboardingRepoMock,
        [Frozen] Mock<ITenantRepository> tenantRepoMock,
        OnboardingService service)
    {
        // Arrange
        var request = new OnboardingRequest
        {
            TenantName = "My Tenant",
        };
        using var cts = new CancellationTokenSource();

        onboardingRepoMock
            .Setup(r => r.UserAlreadyHasTenantAsync(userId, cts.Token))
            .ReturnsAsync(false);

        // Act
        var response = await service.ExecuteAsync(userId, request, cts.Token);

        tenantRepoMock.Verify(r => r.CreateAsync(
            It.IsAny<Tenant>(),
            It.IsAny<TenantUser>(),
            cts.Token
        ), Times.Once);
    }
}
