using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Ecommerce.Api.Tests.Extensions;

public static class WebApplicationFactoryExtensions
{
    public static HttpClient CreateAuthenticatedClient(
        this WebApplicationFactory<Program> factory)
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

        return client;
    }
}
