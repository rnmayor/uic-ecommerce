using Ecommerce.Application.Admin.Stores.Brands.GetAll;

namespace Ecommerce.Application.Tests.Admin.Stores.Brands.GetAll;

public sealed class GetAllStoreBrandsServiceTests
{
    private readonly Mock<IGetAllStoreBrandsRepository> _repositoryMock;
    private readonly GetAllStoreBrandsService _service;
    public GetAllStoreBrandsServiceTests()
    {
        _repositoryMock = new Mock<IGetAllStoreBrandsRepository>();
        _service = new GetAllStoreBrandsService(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ReturnsStoreBrandList()
    {
        // Arrange
        var storeBrands = new List<StoreBrandDto>
    {
      new()
      {
        BrandId = Guid.NewGuid(),
        Name = "Brand A"
      },
      new()
      {
        BrandId = Guid.NewGuid(),
        Name = "Brand B"
      }
    };

        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(storeBrands);

        // Act
        var response = await _service.HandleAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(2, response.Brands.Count);
        Assert.Same(storeBrands, response.Brands);

        _repositoryMock.Verify(r => r.GetAllAsync(
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ReturnsEmptyList()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<StoreBrandDto>());

        // Act
        var response = await _service.HandleAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Empty(response.Brands);

        _repositoryMock.Verify(r => r.GetAllAsync(
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_PassesCancellationToken()
    {
        using var cts = new CancellationTokenSource();

        _repositoryMock
            .Setup(r => r.GetAllAsync(cts.Token))
            .ReturnsAsync(new List<StoreBrandDto>());

        await _service.HandleAsync(cts.Token);

        _repositoryMock.Verify(r => r.GetAllAsync(cts.Token), Times.Once);
    }
}
