using AutoFixture.Xunit2;
using Ecommerce.Application.Admin.Stores.Brands.GetAll;
using Ecommerce.TestUtils.Attributes;

namespace Ecommerce.Application.Tests.Admin.Stores.Brands.GetAll;

public sealed class GetAllStoreBrandsServiceTests
{
    [Theory, AutoMoqData]
    public async Task HandleAsync_ReturnsStoreBrandList(
        [Frozen] Mock<IGetAllStoreBrandsRepository> repositoryMock,
        GetAllStoreBrandsService service)
    {
        // Arrange
        var storeBrands = new List<StoreBrandDto>
        {
            new() { BrandId = Guid.NewGuid(), Name = "Brand A" },
            new() { BrandId = Guid.NewGuid(), Name = "Brand B" }
        };

        repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(storeBrands);

        // Act
        var response = await service.HandleAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(2, response.Brands.Count);
        Assert.Same(storeBrands, response.Brands);

        repositoryMock.Verify(r => r.GetAllAsync(
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Theory, AutoMoqData]
    public async Task HandleAsync_ReturnsEmptyList(
        [Frozen] Mock<IGetAllStoreBrandsRepository> repositoryMock,
        GetAllStoreBrandsService service
    )
    {
        // Arrange
        repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<StoreBrandDto>());

        // Act
        var response = await service.HandleAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Empty(response.Brands);

        repositoryMock.Verify(r => r.GetAllAsync(
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Theory, AutoMoqData]
    public async Task HandleAsync_PassesCancellationToken(
        [Frozen] Mock<IGetAllStoreBrandsRepository> repositoryMock,
        GetAllStoreBrandsService service
    )
    {
        using var cts = new CancellationTokenSource();

        repositoryMock
            .Setup(r => r.GetAllAsync(cts.Token))
            .ReturnsAsync(new List<StoreBrandDto>());

        await service.HandleAsync(cts.Token);

        repositoryMock.Verify(r => r.GetAllAsync(cts.Token), Times.Once);
    }
}
