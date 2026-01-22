namespace Ecommerce.Application.Common.Identity;

public interface IUserResolver
{
    Task<Guid> ResolveUserIdAsync(string clerkUserId);
}
