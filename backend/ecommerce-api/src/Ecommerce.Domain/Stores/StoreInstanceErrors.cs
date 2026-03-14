using Ecommerce.Domain.Common;
using System.Net;

namespace Ecommerce.Domain.Stores
{
    public static class StoreInstanceErrors
    {
        public static readonly Error TenantRequired = new("store_instance.tenant_required", "Tenant Id is required.", HttpStatusCode.BadRequest);
        public static readonly Error StoreBrandRequired = new("store_instance.store_brand_required", "Store brand ID is required", HttpStatusCode.BadRequest);
        public static readonly Error NameRequired = new("store_instance.store_name_required", "Store display name is required", HttpStatusCode.BadRequest);
    }
}
