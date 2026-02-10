using System.Net;
using System.Net.Http.Json;
using Ecommerce.Api.Tests.Extensions;
using Ecommerce.Api.Tests.Fixtures;
using Ecommerce.Application.Admin.Tenants.Onboarding;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Api.Tests.Controllers.Admin;

public sealed class OnboardingControllerTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<ICreateTenantService> _serviceMock;

    public OnboardingControllerTests(ApiWebApplicationFactory factory)
    {
        _serviceMock = new Mock<ICreateTenantService>();

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped(_ => _serviceMock.Object);
            });
        });
    }

    [Fact]
    public async Task CreateTenant_WhenValidRequest_ReturnsCreated()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient();

        var request = new CreateTenantRequest
        {
            TenantName = "My Tenant"
        };

        var response = new CreateTenantResponse
        {
            TenantId = Guid.NewGuid()
        };

        _serviceMock
            .Setup(s => s.CreateAsync(
                It.IsAny<Guid>(),
                It.IsAny<CreateTenantRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var httpResponse = await client.PostAsJsonAsync(
            "/api/admin/onboarding/tenant",
            request
        );

        // Assert
        Assert.Equal(HttpStatusCode.Created, httpResponse.StatusCode);

        var body = await httpResponse.Content.ReadFromJsonAsync<CreateTenantResponse>();

        Assert.NotNull(body);
        Assert.Equal(response.TenantId, body.TenantId);

        _serviceMock.Verify(s => s.CreateAsync(
            It.IsAny<Guid>(),
            It.IsAny<CreateTenantRequest>(),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task CreateTenant_WhenInvalidRequest_Returns400()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient();

        var request = new CreateTenantRequest
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

        _serviceMock.Verify(s => s.CreateAsync(
            It.IsAny<Guid>(),
            It.IsAny<CreateTenantRequest>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);
    }

    [Fact]
    public async Task CreateTenant_WhenUserIsUnauthenticated_Returns401()
    {
        // Arrange
        var client = _factory.CreateClient();

        var request = new CreateTenantRequest
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

        _serviceMock.Verify(s => s.CreateAsync(
            It.IsAny<Guid>(),
            It.IsAny<CreateTenantRequest>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);
    }
}
