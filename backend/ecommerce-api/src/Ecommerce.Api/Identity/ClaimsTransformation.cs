using System.Security.Claims;
using Ecommerce.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authentication;

namespace Ecommerce.Api.Identity;

public sealed class ClaimsTransformation : IClaimsTransformation
{
    private readonly IUserResolver _userResolver;
    public ClaimsTransformation(IUserResolver userResolver)
    {
        _userResolver = userResolver;
    }
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        // check user is actually authenticated
        if (principal.Identity?.IsAuthenticated != true)
        {
            return principal;
        }
        // checks if we already added the user_id claim
        var identity = (ClaimsIdentity)principal.Identity;
        if (identity.HasClaim(c => c.Type == "user_id"))
        {
            return principal;
        }
        // check external Id provided by Clerk - claim type NameIdentifier or "sub"
        var clerkUserId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(clerkUserId))
        {
            return principal;
        }
        // call the lazy provisioning logic
        var userId = await _userResolver.ResolveUserIdAsync(clerkUserId);
        // add new claim user_id with the internal GUID of the user
        identity.AddClaim(new Claim("user_id", userId.ToString()));

        // returns enriched ClaimsPrincipal to the pipeline
        return principal;
    }
}
