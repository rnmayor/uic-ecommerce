using AutoFixture.Xunit2;
using Ecommerce.Application.Admin.Stores.Brands.Create;
using Ecommerce.Application.Common.Persistence;
using Ecommerce.Domain.Stores;
using Ecommerce.TestUtils.Attributes;
using FluentValidation;
using FluentValidation.Results;

namespace Ecommerce.Application.Tests.Admin.Stores.Brands.Create
{
    public sealed class CreateStoreBrandServiceTests
    {
        [Theory, AutoMoqData]
        public async Task ExecuteAsync_CreatesStoreBrand_WhenStoreBrandDoesNotExist(
            [Frozen] Mock<IValidator<CreateStoreBrandRequest>> validatorMock,
            [Frozen] Mock<IStoreBrandRepository> repositoryMock,
            CreateStoreBrandService service)
        {
            // Arrange
            var request = new CreateStoreBrandRequest
            {
                StoreBrandName = "My Brand"
            };

            validatorMock
                .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            var expectedNormalizedName = StoreBrand.Normalize(request.StoreBrandName);
            repositoryMock
                .Setup(r => r.StoreBrandExistAsync(expectedNormalizedName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await service.ExecuteAsync(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotEqual(Guid.Empty, result.Value.StoreBrandId);

            validatorMock.Verify(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()), Times.Once);

            repositoryMock.Verify(r => r.CreateAsync(
              It.Is<StoreBrand>(b => b.Name == request.StoreBrandName),
              It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Theory, AutoMoqData]
        public async Task ExecuteAsync_ReturnsFailure_WhenValidationFails(
            [Frozen] Mock<IValidator<CreateStoreBrandRequest>> validatorMock,
            [Frozen] Mock<IStoreBrandRepository> repositoryMock,
            CreateStoreBrandService service)
        {
            // Arrange
            var request = new CreateStoreBrandRequest
            {
                StoreBrandName = ""
            };
            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure(nameof(request.StoreBrandName), "Name is required")
            };

            validatorMock
                .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(validationFailures));

            // Act
            var result = await service.ExecuteAsync(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(StoreBrandErrors.ValidationFailed("").Code, result.Error.Code);
            Assert.Equal("Name is required", result.Error.Description);

            validatorMock.Verify(v => v.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()
            ), Times.Once);

            repositoryMock.Verify(r => r.StoreBrandExistAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ), Times.Never);

            repositoryMock.Verify(r => r.CreateAsync(
                It.IsAny<StoreBrand>(),
                It.IsAny<CancellationToken>()
            ), Times.Never);
        }

        [Theory, AutoMoqData]
        public async Task ExecuteAsync_ReturnsFailure_WhenStoreBrandExist(
            [Frozen] Mock<IValidator<CreateStoreBrandRequest>> validatorMock,
            [Frozen] Mock<IStoreBrandRepository> repositoryMock,
            CreateStoreBrandService service)
        {
            // Arrange
            var request = new CreateStoreBrandRequest
            {
                StoreBrandName = "My Brand"
            };

            validatorMock
                .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            var expectedNormalizedName = StoreBrand.Normalize(request.StoreBrandName);
            repositoryMock
                .Setup(r => r.StoreBrandExistAsync(expectedNormalizedName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await service.ExecuteAsync(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(StoreBrandErrors.NameAlreadyExists.Code, result.Error.Code);

            validatorMock.Verify(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()), Times.Once);

            repositoryMock.Verify(r => r.CreateAsync(
                It.IsAny<StoreBrand>(),
                It.IsAny<CancellationToken>()
            ), Times.Never);
        }

        [Theory, AutoMoqData]
        public async Task ExecuteAsync_PassesCancellationToken(
            [Frozen] Mock<IValidator<CreateStoreBrandRequest>> validatorMock,
            [Frozen] Mock<IStoreBrandRepository> repositoryMock,
            CreateStoreBrandService service)
        {
            // Arrange
            var request = new CreateStoreBrandRequest
            {
                StoreBrandName = "My Brand"
            };
            using var cts = new CancellationTokenSource();

            validatorMock
                .Setup(v => v.ValidateAsync(request, cts.Token))
                .ReturnsAsync(new ValidationResult());

            var expectedNormalizedName = StoreBrand.Normalize(request.StoreBrandName);
            repositoryMock
                .Setup(r => r.StoreBrandExistAsync(expectedNormalizedName, cts.Token))
                .ReturnsAsync(false);

            // Act
            var result = await service.ExecuteAsync(request, cts.Token);

            // Assert
            Assert.True(result.IsSuccess);
            validatorMock.Verify(v => v.ValidateAsync(request, cts.Token), Times.Once);

            repositoryMock.Verify(r => r.CreateAsync(
                It.IsAny<StoreBrand>(),
                cts.Token
            ), Times.Once);
        }
    }
}
