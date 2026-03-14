using Ecommerce.Application.Common.Persistence;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Tenants;
using FluentValidation;

namespace Ecommerce.Application.Admin.Tenants.Onboarding
{
    public sealed class OnboardingService : IOnboardingService
    {
        private readonly IValidator<OnboardingRequest> _validator;
        private readonly IOnboardingRepository _onboardingRepo;
        private readonly ITenantRepository _tenantRepo;
        public OnboardingService(IValidator<OnboardingRequest> validator, IOnboardingRepository onboardingRepo, ITenantRepository tenantRepo)
        {
            _validator = validator;
            _onboardingRepo = onboardingRepo;
            _tenantRepo = tenantRepo;
        }
        public async Task<Result<OnboardingResponse>> ExecuteAsync(Guid userId, OnboardingRequest request, CancellationToken ct = default)
        {
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                var firstError = validationResult.Errors.First();
                return TenantErrors.ValidationFailed(firstError.ErrorMessage);
            }

            if (await _onboardingRepo.UserAlreadyHasTenantAsync(userId, ct))
            {
                return TenantErrors.UserOwnsTenant;
            }

            var tenant = Tenant.Created(request.TenantName, userId);
            if (tenant.IsFailure)
            {
                return tenant.Error;
            }

            var tenantUser = new TenantUser(tenant.Value.Id, userId, TenantRoles.Owner);

            await _tenantRepo.CreateAsync(tenant.Value, tenantUser, ct);

            return new OnboardingResponse
            {
                TenantId = tenant.Value.Id
            };
        }
    }
}