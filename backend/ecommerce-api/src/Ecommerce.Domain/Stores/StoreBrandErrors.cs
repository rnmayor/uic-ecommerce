using Ecommerce.Domain.Common;
using System.Net;

namespace Ecommerce.Domain.Stores
{
    public static class StoreBrandErrors
    {
        public static readonly Error NameRequired = new("store_brand.name_required", "Store brand name is required.", HttpStatusCode.BadRequest);

        public static readonly Error NameAlreadyExists = new("store_brand.name_conflict", "Store brand name already exists.", HttpStatusCode.Conflict);

        public static readonly Error NotFound = new("store_brand.name_not_found", "Store brand not found.", HttpStatusCode.NotFound);
        public static Error ValidationFailed(string message) => new("store_brand.validation_failed", message, HttpStatusCode.BadRequest);
    }
}