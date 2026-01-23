namespace Ecommerce.Application.Common.Interfaces;

public interface IUserResolver
{
    Task<Guid> ResolveUserIdAsync(string clerkUserId);
}
