using AutoFixture.Xunit2;
using Ecommerce.Application.Admin.Stores.Brands.Create;
using Ecommerce.Application.Common.Persistence;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Stores;
using Ecommerce.TestUtils.Attributes;

namespace Ecommerce.Application.Tests.Admin.Stores.Brands.Create;

public sealed class CreateStoreBrandServiceTests
{
    [Theory, AutoMoqData]
    public async Task ExecuteAsync_CreatesStoreBrand_WhenStoreBrandDoesNotExist(
        [Frozen] Mock<IStoreBrandRepository> repositoryMock,
        CreateStoreBrandService service)
    {
        // Arrange
        var request = new CreateStoreBrandRequest
        {
            StoreBrandName = "My Brand"
        };
        var expectedNormalizedName = StoreBrand.Normalize(request.StoreBrandName);

        repositoryMock
            .Setup(r => r.StoreBrandExistAsync(expectedNormalizedName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var response = await service.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, response.StoreBrandId);

        repositoryMock.Verify(r => r.CreateAsync(
          It.Is<StoreBrand>(b => b.Name == request.StoreBrandName),
          It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Theory, AutoMoqData]
    public async Task ExecuteAsync_Throws_WhenStoreBrandExist(
        [Frozen] Mock<IStoreBrandRepository> repositoryMock,
        CreateStoreBrandService service)
    {
        // Arrange
        var request = new CreateStoreBrandRequest
        {
            StoreBrandName = "My Brand"
        };
        var expectedNormalizedName = StoreBrand.Normalize(request.StoreBrandName);

        repositoryMock
            .Setup(r => r.StoreBrandExistAsync(expectedNormalizedName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            service.ExecuteAsync(request, CancellationToken.None)
        );

        // Assert
        Assert.Contains($"Store brand {request.StoreBrandName} already exist", ex.Message);

        repositoryMock.Verify(r => r.CreateAsync(
            It.IsAny<StoreBrand>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);
    }

    [Theory, AutoMoqData]
    public async Task ExecuteAsync_PassesCancellationToken(
        [Frozen] Mock<IStoreBrandRepository> repositoryMock,
        CreateStoreBrandService service)
    {
        // Arrange
        var request = new CreateStoreBrandRequest
        {
            StoreBrandName = "My Brand"
        };
        var expectedNormalizedName = StoreBrand.Normalize(request.StoreBrandName);
        using var cts = new CancellationTokenSource();

        repositoryMock
            .Setup(r => r.StoreBrandExistAsync(expectedNormalizedName, cts.Token))
            .ReturnsAsync(false);

        // Act
        var response = await service.ExecuteAsync(request, cts.Token);

        // Assert
        repositoryMock.Verify(r => r.CreateAsync(
            It.IsAny<StoreBrand>(),
            cts.Token
        ), Times.Once);
    }
}
