using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Domain.Users;
using Ecommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Identity
{
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
            var userResult = User.Create(clerkUserId);
            if (userResult.IsFailure)
            {
                throw new InvalidOperationException($"Failed to create user: {clerkUserId}. Reason: {userResult.Error.Description}");
            }

            _context.Users.Add(userResult.Value);
            await _context.SaveChangesAsync();

            return userResult.Value.Id;
        }
    }
}