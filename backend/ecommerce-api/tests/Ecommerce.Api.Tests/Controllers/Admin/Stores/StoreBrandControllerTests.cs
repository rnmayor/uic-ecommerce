using System.Net;
using System.Net.Http.Json;
using Ecommerce.Api.Tests.Extensions;
using Ecommerce.Api.Tests.Fixtures;
using Ecommerce.Application.Admin.Stores.Brands;
using Ecommerce.Application.Stores.Brands;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Api.Tests.Controllers.Admin.Stores;

public sealed class StoreBrandControllerTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IStoreBrandService> _serviceMock;
    public StoreBrandControllerTests(ApiWebApplicationFactory factory)
    {
        _serviceMock = new Mock<IStoreBrandService>();

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped(_ => _serviceMock.Object);
            });
        });
    }

    [Fact]
    public async Task GetAllAsync_ReturnsStoreBrandList()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient();

        var response = new StoreBrandResponse
        {
            Brands = [
                new StoreBrandDto
                {
                    BrandId = Guid.NewGuid(),
                    Name = "Store Brand A"
                },
                new StoreBrandDto
                {
                    BrandId = Guid.NewGuid(),
                    Name = "Store Brand B"
                }
            ]
        };

        _serviceMock
            .Setup(s => s.GetAllAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var httpResponse = await client.GetAsync("/api/admin/store-brands");

        // Assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);

        var body = await httpResponse.Content.ReadFromJsonAsync<StoreBrandResponse>();

        Assert.NotNull(body);
        Assert.Equal(2, body.Brands.Count);

        _serviceMock.Verify(s => s.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient();

        _serviceMock
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StoreBrandResponse());

        // Act
        var response = await client.GetAsync("/api/admin/store-brands");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<StoreBrandResponse>();

        Assert.NotNull(body);
        Assert.Empty(body.Brands);

        _serviceMock.Verify(s => s.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WhenUserIsUnauthenticated_Returns401()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/admin/store-brands");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        _serviceMock.Verify(s => s.GetAllAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
