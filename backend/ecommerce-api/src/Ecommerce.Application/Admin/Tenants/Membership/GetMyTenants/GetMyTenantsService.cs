namespace Ecommerce.Application.Admin.Tenants.Membership.GetMyTenants;

public interface IGetMyTenantsRepository
{
    Task<IReadOnlyList<MyTenantDto>> GetTenantsForUserAsync(Guid userId, CancellationToken ct = default);
}

public sealed class GetMyTenantsService : IGetMyTenantsService
{
    private readonly IGetMyTenantsRepository _repository;
    public GetMyTenantsService(IGetMyTenantsRepository repository)
    {
        _repository = repository;
    }
    public async Task<MyTenantsResponse> HandleAsync(Guid userId, CancellationToken ct = default)
    {
        var tenants = await _repository.GetTenantsForUserAsync(userId, ct);

        return new MyTenantsResponse
        {
            Tenants = tenants
        };
    }
}
