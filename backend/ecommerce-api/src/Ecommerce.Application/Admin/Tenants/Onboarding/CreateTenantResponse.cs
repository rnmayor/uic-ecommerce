namespace Ecommerce.Application.Admin.Tenants.Onboarding;

public sealed class CreateTenantResponse
{
    public Guid TenantId { get; init; }
    public Guid StoreId { get; init; }
}
