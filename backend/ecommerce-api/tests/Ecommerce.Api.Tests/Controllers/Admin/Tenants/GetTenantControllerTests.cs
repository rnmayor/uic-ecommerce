using Ecommerce.Api.Tests.Extensions;
using Ecommerce.Api.Tests.Fixtures;
using Ecommerce.Application.Admin.Tenants.Queries.GetTenant;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Tenants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Ecommerce.Api.Tests.Controllers.Admin.Tenants
{
    public sealed class GetTenantControllerTests : IClassFixture<ApiWebApplicationFactory>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly Mock<IGetTenantService> _serviceMock;
        public GetTenantControllerTests(ApiWebApplicationFactory factory)
        {
            _serviceMock = new Mock<IGetTenantService>();

            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddScoped(_ => _serviceMock.Object);
                });
            });
        }

        [Fact]
        public async Task HandleAsync_WhenTenantExists_ReturnsTenant()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var client = _factory.CreateTenantClient(tenantId);
            var slug = "tenant-a";

            var response = new GetTenantResponse
            {
                Tenant = new GetTenantDTO
                {
                    Id = tenantId,
                    Name = "Tenant A",
                    Slug = slug
                },
                Stores = new List<GetTenantStoreDTO>
                {
                    new() { Id = Guid.NewGuid(), DisplayName = "Store 1", StoreBrandName = "Brand 1"},
                    new() { Id = Guid.NewGuid(), DisplayName = "Store 2", StoreBrandName = "Brand 1"},
                    new() { Id = Guid.NewGuid(), DisplayName = "Store 3", StoreBrandName = "Brand 2"},
                }
            };

            _serviceMock
                .Setup(s => s.HandleAsync(slug, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<GetTenantResponse>.Success(response));

            // Act
            var httpResponse = await client.GetAsync($"/api/admin/tenants/{slug}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);

            var body = await httpResponse.Content.ReadFromJsonAsync<GetTenantResponse>();

            Assert.NotNull(body);
            Assert.Equal(response.Tenant.Id, body.Tenant.Id);
            Assert.Equal(slug, body.Tenant.Slug);
            Assert.Equal(response.Stores.Count, body.Stores.Count);

            _serviceMock.Verify(s => s.HandleAsync(slug, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenSlugIsInvalid_Returns400()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var client = _factory.CreateTenantClient(tenantId);
            var invalidSlug = "---invalid---slug";

            _serviceMock
                .Setup(s => s.HandleAsync(invalidSlug, It.IsAny<CancellationToken>()))
                .ReturnsAsync(TenantErrors.InvalidSlug);

            // Act
            var httpResponse = await client.GetAsync($"/api/admin/tenants/{invalidSlug}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);

            var problem = await httpResponse.Content.ReadFromJsonAsync<ProblemDetails>();
            Assert.NotNull(problem);
            Assert.Equal(TenantErrors.InvalidSlug.Code, problem.Type);

            _serviceMock.Verify(s => s.HandleAsync(invalidSlug, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenTenantNotFound_Returns404()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var client = _factory.CreateTenantClient(tenantId);
            var slug = "tenant-a";

            _serviceMock
                .Setup(s => s.HandleAsync(slug, It.IsAny<CancellationToken>()))
                .ReturnsAsync(TenantErrors.TenantNotFound);

            // Act
            var httpResponse = await client.GetAsync($"/api/admin/tenants/{slug}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);

            var problem = await httpResponse.Content.ReadFromJsonAsync<ProblemDetails>();
            Assert.NotNull(problem);
            Assert.Equal(TenantErrors.TenantNotFound.Code, problem.Type);

            _serviceMock.Verify(s => s.HandleAsync(slug, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenTenantHeaderMissing_Returns400()
        {
            // Arrange
            var client = _factory.CreateAuthenticatedClient();
            var slug = "tenant-a";

            var response = new GetTenantResponse
            {
                Tenant = new GetTenantDTO
                {
                    Id = Guid.NewGuid(),
                    Name = "Tenant A",
                    Slug = slug
                },
                Stores = new List<GetTenantStoreDTO>
                {
                    new() { Id = Guid.NewGuid(), DisplayName = "Store 1", StoreBrandName = "Brand 1"},
                    new() { Id = Guid.NewGuid(), DisplayName = "Store 2", StoreBrandName = "Brand 1"},
                    new() { Id = Guid.NewGuid(), DisplayName = "Store 3", StoreBrandName = "Brand 2"},
                }
            };

            _serviceMock
                .Setup(s => s.HandleAsync(slug, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<GetTenantResponse>.Success(response));

            // Act
            var httpResponse = await client.GetAsync($"/api/admin/tenants/{slug}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);

            var problem = await httpResponse.Content.ReadFromJsonAsync<ProblemDetails>();
            Assert.NotNull(problem);
            Assert.Equal("api.invalid_tenant_context", problem.Type);

            _serviceMock.Verify(s => s.HandleAsync(slug, It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenServiceFails_ReturnsProblemDetails()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var client = _factory.CreateTenantClient(tenantId);
            var slug = "tenant-a";

            var expectedError = new Error("db.timeout", "Database timeout.", HttpStatusCode.ServiceUnavailable);

            _serviceMock
                .Setup(s => s.HandleAsync(
                    slug,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedError);

            // Act
            var httpResponse = await client.GetAsync($"/api/admin/tenants/{slug}");

            // Assert
            Assert.Equal(HttpStatusCode.ServiceUnavailable, httpResponse.StatusCode);

            var problem = await httpResponse.Content.ReadFromJsonAsync<ProblemDetails>();
            Assert.NotNull(problem);
            Assert.Equal("db.timeout", problem.Type);
            Assert.Equal("DB TIMEOUT", problem.Title);

            _serviceMock.Verify(s => s.HandleAsync(
                slug,
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenUserIsUnauthenticated_Returns401()
        {
            // Arrange
            var client = _factory.CreateClient();
            var slug = "tenant-a";

            // Act
            var httpResponse = await client.GetAsync($"/api/admin/tenants/{slug}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, httpResponse.StatusCode);

            _serviceMock.Verify(s => s.HandleAsync(
                slug,
                It.IsAny<CancellationToken>()
            ), Times.Never);
        }
    }
}
