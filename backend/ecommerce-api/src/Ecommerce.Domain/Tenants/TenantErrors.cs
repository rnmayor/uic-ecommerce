using Ecommerce.Domain.Common;
using System.Net;

namespace Ecommerce.Domain.Tenants
{
    public static class TenantErrors
    {
        public static readonly Error NameRequired = new("tenant.name_required", "Tenant name is required.", HttpStatusCode.BadRequest);
        public static readonly Error OwnerRequired = new("tenant.owner_required", "Owner user ID is required.", HttpStatusCode.BadRequest);
        public static readonly Error UserOwnsTenant = new("tenant.user_owns_tenant", "User already owns a tenant.", HttpStatusCode.BadRequest);
        public static readonly Error NameAlreadyExists = new("tenant.name_conflict", "Tenant name already exists.", HttpStatusCode.BadRequest);
        public static readonly Error TenantNotFound = new("tenant.not_found", "Tenant not found", HttpStatusCode.NotFound);
        public static readonly Error InvalidSlug = new("tenant.invalid_slug", "Invalid slug", HttpStatusCode.BadRequest);
        public static Error ValidationFailed(string message) => new("tenant.validation_failed", message, HttpStatusCode.BadRequest);
    }
}
