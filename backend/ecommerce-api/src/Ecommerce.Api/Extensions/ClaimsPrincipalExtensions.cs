using System.Security.Claims;

namespace Ecommerce.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var id = user.FindFirst("user_id")?.Value;
        if (string.IsNullOrEmpty(id))
        {
            throw new UnauthorizedAccessException();
        }

        return Guid.Parse(id);
    }
}
