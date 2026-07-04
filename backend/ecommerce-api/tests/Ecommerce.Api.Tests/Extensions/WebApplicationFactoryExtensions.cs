using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Ecommerce.Api.Tests.Extensions;

public static class WebApplicationFactoryExtensions
{
    private const string TenantHeader = "X-Tenant-Id";
    public static HttpClient CreateAuthenticatedClient(
        this WebApplicationFactory<Program> factory)
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

        return client;
    }

    public static HttpClient CreateTenantClient(
        this WebApplicationFactory<Program> factory,
        Guid tenantId)
    {
        var client = factory.CreateAuthenticatedClient();
        client.DefaultRequestHeaders.Add(TenantHeader, tenantId.ToString());

        return client;
    }
}
