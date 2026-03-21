using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Application.Common.Validation;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Tenants;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Admin.Tenants.Queries.GetTenant
{
    public sealed class GetTenantService : IGetTenantService
    {
        private readonly IGetTenantRepository _repository;
        private readonly ITenantContext _tenantContext;
        private readonly ILogger<GetTenantService> _logger;
        public GetTenantService(IGetTenantRepository repository, ITenantContext tenantContext, ILogger<GetTenantService> logger)
        {
            _repository = repository;
            _tenantContext = tenantContext;
            _logger = logger;
        }

        public async Task<Result<GetTenantResponse>> HandleAsync(string slug, CancellationToken ct = default)
        {
            if (!SlugValidator.IsCanonical(slug))
            {
                return TenantErrors.InvalidSlug;
            }

            var tenantId = _tenantContext.TenantId;
            var result = await _repository.GetTenantAsync(tenantId, ct);
            if (result == null)
            {
                return TenantErrors.TenantNotFound;
            }

            if (result.Tenant.Slug != slug)
            {
                // Someone might be tampering with URLs.
                _logger.LogWarning(
                    "Security: Potential URL tampering. Tenant ID {TenantId} mismatch. Expected {ActualSlug} but requested {RequestedSlug}.",
                    tenantId,
                    result.Tenant.Slug,
                    slug);

                return TenantErrors.TenantNotFound;
            }

            return result;
        }
    }
}
