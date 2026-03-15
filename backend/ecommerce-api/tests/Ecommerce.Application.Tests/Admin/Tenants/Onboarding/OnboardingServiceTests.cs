using AutoFixture.Xunit2;
using Ecommerce.Application.Admin.Tenants.Onboarding;
using Ecommerce.Application.Common.Persistence;
using Ecommerce.Domain.Tenants;
using Ecommerce.TestUtils.Attributes;
using FluentValidation;
using FluentValidation.Results;

namespace Ecommerce.Application.Tests.Admin.Tenants.Onboarding
{
    public sealed class OnboardingServiceTests
    {
        [Theory, AutoMoqData]
        public async Task ExecuteAsync_CreatesTenantAndOwner_WhenUserDoesNotOwnTenant(
            Guid userId,
            [Frozen] Mock<IValidator<OnboardingRequest>> validatorMock,
            [Frozen] Mock<IOnboardingRepository> onboardingRepoMock,
            [Frozen] Mock<ITenantRepository> tenantRepoMock,
            OnboardingService service)
        {
            // Arrange
            var request = new OnboardingRequest
            {
                TenantName = "My Tenant",
            };

            validatorMock
                .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            onboardingRepoMock
                .Setup(r => r.UserAlreadyHasTenantAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await service.ExecuteAsync(userId, request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotEqual(Guid.Empty, result.Value.TenantId);

            validatorMock.Verify(v => v.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()
            ), Times.Once);

            onboardingRepoMock.Verify(r => r.UserAlreadyHasTenantAsync(
                userId,
                It.IsAny<CancellationToken>()
            ), Times.Once);

            tenantRepoMock.Verify(r => r.CreateAsync(
                It.Is<Tenant>(t =>
                    t.Name == request.TenantName &&
                    t.OwnerUserId == userId
                ),
                It.Is<TenantUser>(tu =>
                    tu.TenantId == result.Value.TenantId &&
                    tu.UserId == userId &&
                    tu.Role == TenantRoles.Owner
                ),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Theory, AutoMoqData]
        public async Task ExecuteAsync_ReturnsFailure_WhenValidationFails(
            Guid userId,
            [Frozen] Mock<IValidator<OnboardingRequest>> validatorMock,
            [Frozen] Mock<IOnboardingRepository> onboardingRepoMock,
            [Frozen] Mock<ITenantRepository> tenantRepoMock,
            OnboardingService service)
        {
            // Arrange
            var request = new OnboardingRequest
            {
                TenantName = ""
            };
            var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure(nameof(request.TenantName), "Tenant name is required")
        };

            validatorMock
                .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(validationFailures));

            // Act
            var result = await service.ExecuteAsync(userId, request, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(TenantErrors.ValidationFailed("").Code, result.Error.Code);
            Assert.Equal("Tenant name is required", result.Error.Description);

            validatorMock.Verify(v => v.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()
            ), Times.Once);

            onboardingRepoMock.Verify(r => r.UserAlreadyHasTenantAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()
            ), Times.Never);

            tenantRepoMock.Verify(r => r.CreateAsync(
                It.IsAny<Tenant>(),
                It.IsAny<TenantUser>(),
                It.IsAny<CancellationToken>()
            ), Times.Never);
        }

        [Theory, AutoMoqData]
        public async Task ExecuteAsync_Throws_WhenUserAlreadyOwnsTenant(
            Guid userId,
            [Frozen] Mock<IValidator<OnboardingRequest>> validatorMock,
            [Frozen] Mock<IOnboardingRepository> onboardingRepoMock,
            [Frozen] Mock<ITenantRepository> tenantRepoMock,
            OnboardingService service)
        {
            // Arrange
            var request = new OnboardingRequest
            {
                TenantName = "My Tenant",
            };

            validatorMock
                .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            onboardingRepoMock
                .Setup(r => r.UserAlreadyHasTenantAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await service.ExecuteAsync(userId, request, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(TenantErrors.UserOwnsTenant.Code, result.Error.Code);

            validatorMock.Verify(v => v.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()
            ), Times.Once);

            onboardingRepoMock.Verify(r => r.UserAlreadyHasTenantAsync(
                userId,
                It.IsAny<CancellationToken>()
            ), Times.Once);

            tenantRepoMock.Verify(r => r.CreateAsync(
                It.IsAny<Tenant>(),
                It.IsAny<TenantUser>(),
                It.IsAny<CancellationToken>()
            ), Times.Never);
        }

        [Theory, AutoMoqData]
        public async Task ExecuteAsync_PassesCancellationToken(
            Guid userId,
            [Frozen] Mock<IValidator<OnboardingRequest>> validatorMock,
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

            validatorMock
                .Setup(v => v.ValidateAsync(request, cts.Token))
                .ReturnsAsync(new ValidationResult());

            onboardingRepoMock
                .Setup(r => r.UserAlreadyHasTenantAsync(userId, cts.Token))
                .ReturnsAsync(false);

            // Act
            var result = await service.ExecuteAsync(userId, request, cts.Token);

            // Assert
            Assert.True(result.IsSuccess);
            validatorMock.Verify(v => v.ValidateAsync(
                request,
                cts.Token
            ), Times.Once);

            onboardingRepoMock.Verify(r => r.UserAlreadyHasTenantAsync(
                It.IsAny<Guid>(),
                cts.Token
            ), Times.Once);

            tenantRepoMock.Verify(r => r.CreateAsync(
                It.IsAny<Tenant>(),
                It.IsAny<TenantUser>(),
                cts.Token
            ), Times.Once);
        }
    }
}
