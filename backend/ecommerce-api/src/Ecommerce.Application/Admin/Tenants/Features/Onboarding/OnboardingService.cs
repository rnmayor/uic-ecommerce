using Ecommerce.Domain.Common;
using Ecommerce.Domain.Tenants;
using FluentValidation;

namespace Ecommerce.Application.Admin.Tenants.Features.Onboarding
{
    public sealed class OnboardingService : IOnboardingService
    {
        private readonly IValidator<OnboardingRequest> _validator;
        private readonly IOnboardingRepository _onboardingRepo;
        public OnboardingService(IValidator<OnboardingRequest> validator, IOnboardingRepository onboardingRepo)
        {
            _validator = validator;
            _onboardingRepo = onboardingRepo;
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

            var tenant = Tenant.Create(request.TenantName, userId);
            if (tenant.IsFailure)
            {
                return tenant.Error;
            }

            var tenantUser = TenantUser.Create(tenant.Value.Id, userId, TenantRoles.Owner);
            if (tenantUser.IsFailure)
            {
                return tenantUser.Error;
            }

            await _onboardingRepo.CreateTenantWithOwnerAsync(tenant.Value, tenantUser.Value, ct);

            return new OnboardingResponse
            {
                TenantId = tenant.Value.Id
            };
        }
    }
}