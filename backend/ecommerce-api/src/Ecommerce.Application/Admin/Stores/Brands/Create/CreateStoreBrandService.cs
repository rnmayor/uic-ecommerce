using Ecommerce.Application.Common.Persistence;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Stores;
using FluentValidation;

namespace Ecommerce.Application.Admin.Stores.Brands.Create
{
    public sealed class CreateStoreBrandService : ICreateStoreBrandService
    {
        private readonly IValidator<CreateStoreBrandRequest> _validator;
        private readonly IStoreBrandRepository _repository;
        public CreateStoreBrandService(IValidator<CreateStoreBrandRequest> validator, IStoreBrandRepository repository)
        {
            _validator = validator;
            _repository = repository;
        }
        public async Task<Result<CreateStoreBrandResponse>> ExecuteAsync(CreateStoreBrandRequest request, CancellationToken ct = default)
        {
            // Manual validation base on CreateStoreBrandRequestValidator
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                var firstError = validationResult.Errors.First();
                return StoreBrandErrors.ValidationFailed(firstError.ErrorMessage);
            }

            // Database validation, check if normalized name already exists
            var normalizedName = StoreBrand.Normalize(request.StoreBrandName);
            if (await _repository.StoreBrandExistAsync(normalizedName, ct))
            {
                return StoreBrandErrors.NameAlreadyExists;
            }

            // Create store brand and validate domain rules
            var storeBrand = StoreBrand.Create(request.StoreBrandName);
            if (storeBrand.IsFailure)
            {
                return storeBrand.Error;
            }

            // Persist to database
            await _repository.CreateAsync(storeBrand.Value, ct);

            return new CreateStoreBrandResponse
            {
                StoreBrandId = storeBrand.Value.Id
            };
        }
    }
}