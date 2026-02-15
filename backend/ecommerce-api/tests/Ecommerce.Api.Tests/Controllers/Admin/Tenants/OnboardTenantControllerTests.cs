using System.Net;
using System.Net.Http.Json;
using Ecommerce.Api.Tests.Extensions;
using Ecommerce.Api.Tests.Fixtures;
using Ecommerce.Application.Admin.Tenants.Onboarding;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Api.Tests.Controllers.Admin.Tenants;

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
    public async Task Handle_WhenValidRequest_ReturnsCreated()
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
            .ReturnsAsync(response);

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
    public async Task Handle_WhenInvalidRequest_Returns400()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient();

        var request = new OnboardingRequest
        {
            TenantName = ""
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/admin/onboarding/tenant",
            request
        );

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        _serviceMock.Verify(s => s.ExecuteAsync(
            It.IsAny<Guid>(),
            It.IsAny<OnboardingRequest>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserIsUnauthenticated_Returns401()
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
