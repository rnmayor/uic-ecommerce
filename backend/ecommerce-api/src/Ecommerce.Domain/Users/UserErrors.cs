using Ecommerce.Domain.Common;
using System.Net;

namespace Ecommerce.Domain.Users
{
    public static class UserErrors
    {
        public static readonly Error ClerkUserRequired = new("user.clerk_user_required", "Clerk user ID is required", HttpStatusCode.BadRequest);
    }
}
