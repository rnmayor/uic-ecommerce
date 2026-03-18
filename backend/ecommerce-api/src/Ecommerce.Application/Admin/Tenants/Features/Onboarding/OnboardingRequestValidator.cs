using FluentValidation;

namespace Ecommerce.Application.Admin.Tenants.Features.Onboarding
{
    public sealed class OnboardingRequestValidator : AbstractValidator<OnboardingRequest>
    {
        public OnboardingRequestValidator()
        {
            RuleFor(x => x.TenantName).NotEmpty().MaximumLength(100);
        }
    }
}