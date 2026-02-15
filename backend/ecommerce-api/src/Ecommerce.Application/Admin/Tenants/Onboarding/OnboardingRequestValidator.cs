using FluentValidation;

namespace Ecommerce.Application.Admin.Tenants.Onboarding;

public sealed class OnboardingRequestValidator : AbstractValidator<OnboardingRequest>
{
    public OnboardingRequestValidator()
    {
        RuleFor(x => x.TenantName).NotEmpty().MaximumLength(100);
    }
}
