using Ecommerce.Api.Tests.Extensions;
using Ecommerce.Api.Tests.Fixtures;
using Ecommerce.Application.Admin.Tenants.Queries.GetMyTenants;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Tenants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Ecommerce.Api.Tests.Controllers.Admin.Tenants
{
    public sealed class GetMyTenantsControllerTests : IClassFixture<ApiWebApplicationFactory>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly Mock<IGetMyTenantsService> _serviceMock;
        public GetMyTenantsControllerTests(ApiWebApplicationFactory factory)
        {
            _serviceMock = new Mock<IGetMyTenantsService>();

            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddScoped(_ => _serviceMock.Object);
                });
            });
        }

        [Fact]
        public async Task HandleAsync_WhenUserHasTenants_ReturnsTenantList()
        {
            // Arrange
            var client = _factory.CreateAuthenticatedClient();

            var response = new MyTenantsResponse
            {
                Tenants = [
                    new MyTenantDTO
                {
                    TenantId = Guid.NewGuid(),
                    Name = "Tenant A",
                    IsOwner = true,
                    Role = TenantRoles.Owner
                },
                new MyTenantDTO
                {
                    TenantId = Guid.NewGuid(),
                    Name = "Tenant B",
                    IsOwner = false,
                    Role = TenantRoles.Admin
                }
                ]
            };

            _serviceMock
                .Setup(s => s.HandleAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<MyTenantsResponse>.Success(response));

            // Act
            var httpResponse = await client.GetAsync("/api/admin/me/tenants");

            // Assert
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);

            var body = await httpResponse.Content.ReadFromJsonAsync<MyTenantsResponse>();

            Assert.NotNull(body);
            Assert.Equal(2, body.Tenants.Count);
            Assert.True(body.HasTenant);

            _serviceMock.Verify(s => s.HandleAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenUserHasNoTenants_ReturnsEmptyList()
        {
            // Arrange
            var client = _factory.CreateAuthenticatedClient();

            _serviceMock
                .Setup(s => s.HandleAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<MyTenantsResponse>.Success(new MyTenantsResponse()));

            // Act
            var response = await client.GetAsync("/api/admin/me/tenants");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var body = await response.Content.ReadFromJsonAsync<MyTenantsResponse>();

            Assert.NotNull(body);
            Assert.Empty(body.Tenants);
            Assert.False(body.HasTenant);

            _serviceMock.Verify(s => s.HandleAsync(
                It.IsAny<Guid>(),
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
                .Setup(s => s.HandleAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedError);

            // Act
            var response = await client.GetAsync("/api/admin/me/tenants");

            // Assert
            Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);

            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            Assert.NotNull(problem);
            Assert.Equal("db.timeout", problem.Type);
            Assert.Equal("DB TIMEOUT", problem.Title);

            _serviceMock.Verify(s => s.HandleAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenUserIsUnauthenticated_Returns401()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/admin/me/tenants");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            _serviceMock.Verify(s => s.HandleAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()
            ), Times.Never);
        }
    }
}