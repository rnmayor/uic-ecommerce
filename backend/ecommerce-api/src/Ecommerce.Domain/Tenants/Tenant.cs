using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Tenants
{
    public class Tenant : Entity
    {
        public string Name { get; private set; } = default!;
        public Guid OwnerUserId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private Tenant() { } // For EF

        public static Result<Tenant> Created(string name, Guid ownerUserId)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return TenantErrors.NameRequired;
            }
            if (ownerUserId == Guid.Empty)
            {
                return TenantErrors.OwnerRequired;
            }

            var now = DateTime.UtcNow;
            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = name.Trim(),
                OwnerUserId = ownerUserId,
                CreatedAt = now,
                UpdatedAt = now,
            };

            return tenant;
        }
    }
}