using Ecommerce.Domain.Common;

namespace Ecommerce.Application.Admin.Tenants.Queries.GetMyTenants
{
    public sealed class GetMyTenantsService : IGetMyTenantsService
    {
        private readonly IGetTenantsForUserRepository _repository;
        public GetMyTenantsService(IGetTenantsForUserRepository repository)
        {
            _repository = repository;
        }
        public async Task<Result<MyTenantsResponse>> HandleAsync(Guid userId, CancellationToken ct = default)
        {
            var result = await _repository.GetTenantsForUserAsync(userId, ct);
            if (result.IsFailure)
            {
                return result.Error;
            }

            return new MyTenantsResponse
            {
                Tenants = result.Value
            };
        }
    }
}