using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Stores
{
    public class StoreInstance : TenantEntity
    {
        public Guid StoreBrandId { get; private set; }
        public StoreBrand StoreBrand { get; private set; } = default!;
        public string DisplayName { get; private set; } = default!;
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        private StoreInstance() { } // For EF
        public static Result<StoreInstance> Create(Guid tenantId, Guid storeBrandId, string displayName)
        {
            if (tenantId == Guid.Empty)
            {
                return StoreInstanceErrors.TenantRequired;
            }
            if (storeBrandId == Guid.Empty)
            {
                return StoreInstanceErrors.StoreBrandRequired;
            }
            if (string.IsNullOrWhiteSpace(displayName))
            {
                return StoreInstanceErrors.NameRequired;
            }

            var now = DateTime.UtcNow;
            var store = new StoreInstance
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                StoreBrandId = storeBrandId,
                DisplayName = displayName.Trim(),
                CreatedAt = now,
                UpdatedAt = now
            };

            return store;
        }
    }
}
