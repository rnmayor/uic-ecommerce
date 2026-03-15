using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Tenants
{
    public static class TenantErrors
    {
        public static readonly Error NameRequired = new("tenant.name_required", "Tenant name is required.", System.Net.HttpStatusCode.BadRequest);
        public static readonly Error OwnerRequired = new("tenant.owner_required", "Owner user ID is required.", System.Net.HttpStatusCode.BadRequest);
        public static readonly Error UserOwnsTenant = new("tenant.user_owns_tenant", "User already owns a tenant.", System.Net.HttpStatusCode.BadRequest);
        public static Error ValidationFailed(string message) => new("tenant.validation_failed", message, System.Net.HttpStatusCode.BadRequest);
    }
}
