using Ecommerce.Domain.Common;
using System.Net;

namespace Ecommerce.Domain.Tenants
{
    public static class TenantUserErrors
    {
        public static readonly Error TenantRequired = new("tenant_user.tenant_required", "Tenant ID is required", HttpStatusCode.BadRequest);
        public static readonly Error UserRequired = new("tenant_user.user_required", "User ID is required", HttpStatusCode.BadRequest);
        public static readonly Error RoleRequired = new("tenant_user.role_required", "Role is required", HttpStatusCode.BadRequest);
        public static readonly Error TenantRoleInvalid = new("tenant_user.role_invalid", "Invalid tenant role", HttpStatusCode.BadRequest);
    }
}
