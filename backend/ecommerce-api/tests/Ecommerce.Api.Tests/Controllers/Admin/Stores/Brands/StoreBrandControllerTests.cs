using System.Net;
using System.Net.Http.Json;
using Ecommerce.Api.Tests.Extensions;
using Ecommerce.Api.Tests.Fixtures;
using Ecommerce.Application.Admin.Stores.Brands.Create;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Api.Tests.Controllers.Admin.Stores.Brands;

public sealed class StoreBrandControllerTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<ICreateStoreBrandService> _serviceMock;
    public StoreBrandControllerTests(ApiWebApplicationFactory factory)
    {
        _serviceMock = new Mock<ICreateStoreBrandService>();

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
        {
            services.AddScoped(_ => _serviceMock.Object);
        });
        });
    }

    [Fact]
    public async Task Create_WhenValidRequest_ReturnsCreated()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient();

        var request = new CreateStoreBrandRequest
        {
            StoreBrandName = "My Brand"
        };

        var response = new CreateStoreBrandResponse
        {
            StoreBrandId = Guid.NewGuid()
        };

        _serviceMock
            .Setup(s => s.ExecuteAsync(
                It.IsAny<CreateStoreBrandRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var httpResponse = await client.PostAsJsonAsync(
            "/api/admin/store-brands",
            request
        );

        // Assert
        Assert.Equal(HttpStatusCode.Created, httpResponse.StatusCode);

        var body = await httpResponse.Content.ReadFromJsonAsync<CreateStoreBrandResponse>();

        Assert.NotNull(body);
        Assert.Equal(response.StoreBrandId, body.StoreBrandId);

        _serviceMock.Verify(s => s.ExecuteAsync(
          It.IsAny<CreateStoreBrandRequest>(),
          It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task Create_WhenInvalidRequest_Returns400()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient();

        var request = new CreateStoreBrandRequest
        {
            StoreBrandName = ""
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/admin/store-brands",
            request
        );

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        _serviceMock.Verify(s => s.ExecuteAsync(
            It.IsAny<CreateStoreBrandRequest>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserIsUnauthenticated_Returns401()
    {
        // Arrange
        var client = _factory.CreateClient();

        var request = new CreateStoreBrandRequest
        {
            StoreBrandName = "My Brand"
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/admin/store-brands",
            request
        );

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        _serviceMock.Verify(s => s.ExecuteAsync(
            It.IsAny<CreateStoreBrandRequest>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);
    }
}
