using System.Net;
using System.Net.Http.Json;
using Ecommerce.Api.Tests.Extensions;
using Ecommerce.Api.Tests.Fixtures;
using Ecommerce.Application.Admin.Tenants.Membership;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Api.Tests.Controllers.Admin.Tenants;

public sealed class TenantMembershipControllerTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IMyTenantService> _serviceMock;
    public TenantMembershipControllerTests(ApiWebApplicationFactory factory)
    {
        _serviceMock = new Mock<IMyTenantService>();

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped(_ => _serviceMock.Object);
            });
        });
    }

    [Fact]
    public async Task GetMyTenants_WhenUserHasTenants_ReturnsTenantList()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient();

        var response = new MyTenantResponse
        {
            Tenants = [
                new MyTenantDto
                {
                    TenantId = Guid.NewGuid(),
                    Name = "Tenant A",
                    IsOwner = true
                },
                new MyTenantDto
                {
                    TenantId = Guid.NewGuid(),
                    Name = "Tenant B",
                    IsOwner = false
                }
            ]
        };

        _serviceMock
            .Setup(s => s.GetMyTenantsAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var httpResponse = await client.GetAsync("/api/admin/me/tenants");

        // Assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);

        var body = await httpResponse.Content.ReadFromJsonAsync<MyTenantResponse>();

        Assert.NotNull(body);
        Assert.Equal(2, body.Tenants.Count);
        Assert.True(body.HasTenant);

        _serviceMock.Verify(s => s.GetMyTenantsAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task GetMyTenants_WhenUserHasNoTenants_ReturnsEmptyList()
    {
        // Arrange
        var client = _factory.CreateAuthenticatedClient();

        _serviceMock
            .Setup(s => s.GetMyTenantsAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MyTenantResponse());

        // Act
        var response = await client.GetAsync("/api/admin/me/tenants");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<MyTenantResponse>();

        Assert.NotNull(body);
        Assert.Empty(body.Tenants);
        Assert.False(body.HasTenant);
    }

    [Fact]
    public async Task GetMyTenants_WhenUserIsUnauthenticated_Returns401()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/admin/me/tenants");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        _serviceMock.Verify(s => s.GetMyTenantsAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);
    }
}
