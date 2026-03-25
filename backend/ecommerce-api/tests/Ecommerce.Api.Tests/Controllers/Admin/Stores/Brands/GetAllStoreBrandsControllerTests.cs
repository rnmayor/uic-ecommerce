using Ecommerce.Api.Tests.Extensions;
using Ecommerce.Api.Tests.Fixtures;
using Ecommerce.Application.Admin.Stores.Brands.Queries.GetAllStoreBrands;
using Ecommerce.Domain.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Ecommerce.Api.Tests.Controllers.Admin.Stores.Brands
{
    public sealed class GetAllStoreBrandsControllerTests : IClassFixture<ApiWebApplicationFactory>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly Mock<IGetAllStoreBrandsService> _serviceMock;
        public GetAllStoreBrandsControllerTests(ApiWebApplicationFactory factory)
        {
            _serviceMock = new Mock<IGetAllStoreBrandsService>();

            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddScoped(_ => _serviceMock.Object);
                });
            });
        }

        [Fact]
        public async Task HandleAsync_ReturnsStoreBrandList()
        {
            // Arrange
            var client = _factory.CreateAuthenticatedClient();

            var response = new StoreBrandsResponse
            {
                Brands = [
                    new StoreBrandDTO
                {
                    BrandId = Guid.NewGuid(),
                    Name = "Store Brand A"
                },
                new StoreBrandDTO
                {
                    BrandId = Guid.NewGuid(),
                    Name = "Store Brand B"
                }
                ],
                Total = 100
            };

            _serviceMock
                .Setup(s => s.HandleAsync(
                    It.Is<GetAllBrandsQuery>(q => q.Skip == 5 && q.Limit == 10 && q.Search == "nike"),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<StoreBrandsResponse>.Success(response));

            // Act
            var httpResponse = await client.GetAsync("/api/admin/store-brands?skip=5&limit=10&search=nike");

            // Assert
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);

            var body = await httpResponse.Content.ReadFromJsonAsync<StoreBrandsResponse>();

            Assert.NotNull(body);
            Assert.Equal(2, body.Brands.Count);
            Assert.Equal(100, body.Total);

            _serviceMock.Verify(s => s.HandleAsync(
                It.Is<GetAllBrandsQuery>(q => q.Skip == 5),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ReturnsEmptyList()
        {
            // Arrange
            var client = _factory.CreateAuthenticatedClient();

            _serviceMock
                .Setup(s => s.HandleAsync(It.IsAny<GetAllBrandsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<StoreBrandsResponse>.Success(new StoreBrandsResponse { Brands = [], Total = 0 }));

            // Act
            var httpResponse = await client.GetAsync("/api/admin/store-brands");

            // Assert
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);

            var body = await httpResponse.Content.ReadFromJsonAsync<StoreBrandsResponse>();

            Assert.NotNull(body);
            Assert.Empty(body.Brands);
            Assert.Equal(0, body.Total);

            _serviceMock.Verify(s => s.HandleAsync(
                It.IsAny<GetAllBrandsQuery>(),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenServiceFails_ReturnsProblemDetails()
        {
            // Arrange
            var client = _factory.CreateAuthenticatedClient();
            var expectedError = new Error("db.timeout", "Database timeout.", HttpStatusCode.ServiceUnavailable);

            _serviceMock
                .Setup(s => s.HandleAsync(It.IsAny<GetAllBrandsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<StoreBrandsResponse>.Failure(expectedError));

            // Act
            var httpResponse = await client.GetAsync("/api/admin/store-brands");

            // Assert
            Assert.Equal(HttpStatusCode.ServiceUnavailable, httpResponse.StatusCode);

            var problem = await httpResponse.Content.ReadFromJsonAsync<ProblemDetails>();
            Assert.NotNull(problem);
            Assert.Equal("db.timeout", problem.Type);
            Assert.Equal("DB TIMEOUT", problem.Title);

            _serviceMock.Verify(s => s.HandleAsync(
                It.IsAny<GetAllBrandsQuery>(),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenUserIsUnauthenticated_Returns401()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var httpResponse = await client.GetAsync("/api/admin/store-brands");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, httpResponse.StatusCode);

            _serviceMock.Verify(s => s.HandleAsync(
                It.IsAny<GetAllBrandsQuery>(),
                It.IsAny<CancellationToken>()
            ), Times.Never);
        }
    }
}