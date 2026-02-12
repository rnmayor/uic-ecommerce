namespace Ecommerce.Application.Admin.Stores.Brands;

public sealed class StoreBrandServiceTests
{
    private readonly Mock<IStoreBrandReadRepository> _readRepositoryMock;
    private readonly IStoreBrandService _service;
    public StoreBrandServiceTests()
    {
        _readRepositoryMock = new Mock<IStoreBrandReadRepository>();
        _service = new StoreBrandService(_readRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsStoreBrandList()
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

        _readRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(storeBrands);

        // Act
        var response = await _service.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(2, response.Brands.Count);
        Assert.Same(storeBrands, response.Brands);

        _readRepositoryMock.Verify(r => r.GetAllAsync(
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList()
    {
        // Arrange
        _readRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<StoreBrandDto>());

        // Act
        var response = await _service.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Empty(response.Brands);

        _readRepositoryMock.Verify(r => r.GetAllAsync(
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_PassesCancellationToken()
    {
        using var cts = new CancellationTokenSource();

        _readRepositoryMock
            .Setup(r => r.GetAllAsync(cts.Token))
            .ReturnsAsync(new List<StoreBrandDto>());

        await _service.GetAllAsync(cts.Token);

        _readRepositoryMock.Verify(r => r.GetAllAsync(cts.Token), Times.Once);
    }
}
