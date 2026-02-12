namespace Ecommerce.Application.Admin.Tenants.Onboarding;

public interface ICreateTenantService
{
    Task<CreateTenantResponse> CreateAsync(
      Guid userId,
      CreateTenantRequest request,
      CancellationToken ct = default
    );
}
