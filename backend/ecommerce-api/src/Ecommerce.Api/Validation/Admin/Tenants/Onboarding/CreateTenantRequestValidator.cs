using Ecommerce.Application.Admin.Tenants.Onboarding;
using FluentValidation;

namespace Ecommerce.Api.Validation.Admin.Tenants.Onboarding;

public sealed class CreateTenantRequestValidator : AbstractValidator<CreateTenantRequest>
{
    public CreateTenantRequestValidator()
    {
        RuleFor(x => x.TenantName).NotEmpty().MaximumLength(100);
    }
}
