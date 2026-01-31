namespace Ecommerce.Application.Admin.Tenants.Membership;

public sealed class MyTenantService : IMyTenantService
{
    private readonly ITenantMembershipReadRepository _repository;
    public MyTenantService(ITenantMembershipReadRepository repository)
    {
        _repository = repository;
    }
    public async Task<MyTenantResponse> GetMyTenantsAsync(Guid userId, CancellationToken ct = default)
    {
        var tenants = await _repository.GetTenantsForUserAsync(userId, ct);

        return new MyTenantResponse
        {
            Tenants = tenants
        };
    }
}
