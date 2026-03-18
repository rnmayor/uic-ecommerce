using Ecommerce.Api.Tests.Extensions;
using Ecommerce.Api.Tests.Fixtures;
using Ecommerce.Application.Admin.Tenants.Features.Onboarding;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Tenants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Ecommerce.Api.Tests.Controllers.Admin.Tenants
{
    public sealed class OnboardTenantControllerTests : IClassFixture<ApiWebApplicationFactory>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly Mock<IOnboardingService> _serviceMock;

        public OnboardTenantControllerTests(ApiWebApplicationFactory factory)
        {
            _serviceMock = new Mock<IOnboardingService>();

            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddScoped(_ => _serviceMock.Object);
                });
            });
        }

        [Fact]
        public async Task HandleAsync_WhenValidRequest_ReturnsCreated()
        {
            // Arrange
            var client = _factory.CreateAuthenticatedClient();

            var request = new OnboardingRequest
            {
                TenantName = "My Tenant"
            };

            var response = new OnboardingResponse
            {
                TenantId = Guid.NewGuid()
            };

            _serviceMock
                .Setup(s => s.ExecuteAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<OnboardingRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<OnboardingResponse>.Success(response));

            // Act
            var httpResponse = await client.PostAsJsonAsync(
                "/api/admin/onboarding/tenant",
                request
            );

            // Assert
            Assert.Equal(HttpStatusCode.Created, httpResponse.StatusCode);

            var body = await httpResponse.Content.ReadFromJsonAsync<OnboardingResponse>();

            Assert.NotNull(body);
            Assert.Equal(response.TenantId, body.TenantId);

            _serviceMock.Verify(s => s.ExecuteAsync(
                It.IsAny<Guid>(),
                It.IsAny<OnboardingRequest>(),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenInvalidRequest_Returns400()
        {
            // Arrange
            var client = _factory.CreateAuthenticatedClient();

            var request = new OnboardingRequest
            {
                TenantName = ""
            };

            _serviceMock
                .Setup(s => s.ExecuteAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<OnboardingRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(TenantErrors.ValidationFailed("Name is required"));

            // Act
            var response = await client.PostAsJsonAsync(
                "/api/admin/onboarding/tenant",
                request
            );

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            Assert.NotNull(problem);
            Assert.Equal(TenantErrors.ValidationFailed("").Code, problem.Type);
            Assert.Equal("TENANT VALIDATION FAILED", problem.Title);

            _serviceMock.Verify(s => s.ExecuteAsync(
                It.IsAny<Guid>(),
                It.IsAny<OnboardingRequest>(),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenUserIsUnauthenticated_Returns401()
        {
            // Arrange
            var client = _factory.CreateClient();

            var request = new OnboardingRequest
            {
                TenantName = "My Tenant"
            };

            // Act
            var response = await client.PostAsJsonAsync(
                "/api/admin/onboarding/tenant",
                request
            );

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            _serviceMock.Verify(s => s.ExecuteAsync(
                It.IsAny<Guid>(),
                It.IsAny<OnboardingRequest>(),
                It.IsAny<CancellationToken>()
            ), Times.Never);
        }
    }
}