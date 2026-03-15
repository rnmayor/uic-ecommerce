using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Stores
{
    public class StoreBrand : Entity
    {
        public string Name { get; private set; } = default!;
        public string NormalizedName { get; private set; } = default!;
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private readonly List<StoreInstance> _storeInstances = [];
        public IReadOnlyCollection<StoreInstance> StoreInstances => _storeInstances.AsReadOnly();

        private StoreBrand() { } // EF

        public static Result<StoreBrand> Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return StoreBrandErrors.NameRequired;
            }

            var now = DateTime.UtcNow;
            var brand = new StoreBrand
            {
                Id = Guid.NewGuid(),
                Name = name.Trim(),
                NormalizedName = Normalize(name),
                CreatedAt = now,
                UpdatedAt = now,
            };

            return brand;

        }

        public static string Normalize(string name) => name.Trim().ToUpperInvariant();
    }
}

