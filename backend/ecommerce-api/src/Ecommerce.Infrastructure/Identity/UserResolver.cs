using Ecommerce.Application.Common.Identity;
using Ecommerce.Domain.Users;
using Ecommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Identity;

public sealed class UserResolver : IUserResolver
{
    private readonly EcommerceDbContext _context;
    public UserResolver(EcommerceDbContext context)
    {
        _context = context;
    }
    public async Task<Guid> ResolveUserIdAsync(string clerkUserId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.ClerkUserId == clerkUserId);

        if (user is not null)
        {
            return user.Id;
        }

        // Create user on first login (idempotent)
        user = new User(clerkUserId);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user.Id;
    }
}
