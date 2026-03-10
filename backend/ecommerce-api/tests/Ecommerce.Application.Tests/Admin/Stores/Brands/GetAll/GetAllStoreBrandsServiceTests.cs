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
        var query = new GetAllBrandsQuery { Skip = 0, Limit = 10 };
        var storeBrands = new List<StoreBrandDTO>
        {
            new() { BrandId = Guid.NewGuid(), Name = "Brand A" },
            new() { BrandId = Guid.NewGuid(), Name = "Brand B" }
        };
        int totalCount = 2;

        repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<GetAllBrandsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((storeBrands, totalCount));

        // Act
        var response = await service.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(totalCount, response.TotalCount);
        Assert.Equal(2, response.Brands.Count);
        Assert.Same(storeBrands, response.Brands);

        repositoryMock.Verify(r => r.GetAllAsync(
            It.Is<GetAllBrandsQuery>(q => q.Limit == 10),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Theory, AutoMoqData]
    public async Task HandleAsync_AppliesHardLimit(
        [Frozen] Mock<IGetAllStoreBrandsRepository> repositoryMock,
        GetAllStoreBrandsService service
    )
    {
        // Arrange
        var query = new GetAllBrandsQuery { Limit = 9999 };
        repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<GetAllBrandsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<StoreBrandDTO>(), 0));

        // Act
        await service.HandleAsync(query, CancellationToken.None);

        repositoryMock.Verify(r => r.GetAllAsync(
            It.Is<GetAllBrandsQuery>(q => q.Limit == 100),
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
        var query = new GetAllBrandsQuery();
        repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<GetAllBrandsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<StoreBrandDTO>(), 0));

        // Act
        var response = await service.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Empty(response.Brands);
        Assert.Equal(0, response.TotalCount);

        repositoryMock.Verify(r => r.GetAllAsync(
            It.IsAny<GetAllBrandsQuery>(),
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
        var query = new GetAllBrandsQuery();

        repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<GetAllBrandsQuery>(), cts.Token))
            .ReturnsAsync((new List<StoreBrandDTO>(), 0));

        await service.HandleAsync(query, cts.Token);

        repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<GetAllBrandsQuery>(), cts.Token), Times.Once);
    }
}
