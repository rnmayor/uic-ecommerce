using Ecommerce.Domain.Common;

namespace Ecommerce.Application.Admin.Tenants.Membership.GetMyTenants
{
    public sealed class GetMyTenantsService : IGetMyTenantsService
    {
        private readonly IGetMyTenantsRepository _repository;
        public GetMyTenantsService(IGetMyTenantsRepository repository)
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