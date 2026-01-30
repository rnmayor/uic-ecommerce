namespace Ecommerce.Application.Admin.Tenants.Onboarding;

public sealed class CreateTenantRequest
{
    public string TenantName { get; init; } = default!;
    public string StoreName { get; init; } = default!;
}
