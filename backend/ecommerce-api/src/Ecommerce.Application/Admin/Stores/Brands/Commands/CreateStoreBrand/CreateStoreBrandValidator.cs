using FluentValidation;

namespace Ecommerce.Application.Admin.Stores.Brands.Commands.CreateStoreBrand
{
    public sealed class CreateStoreBrandRequestValidator : AbstractValidator<CreateStoreBrandRequest>
    {
        public CreateStoreBrandRequestValidator()
        {
            RuleFor(x => x.StoreBrandName).NotEmpty().MaximumLength(100);
        }
    }
}
