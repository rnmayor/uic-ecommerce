using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Users
{
    public class User : Entity
    {
        public string ClerkUserId { get; private set; } = default!;
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        private User() { } // For EF
        public static Result<User> Create(string clerkUserId)
        {
            if (string.IsNullOrWhiteSpace(clerkUserId))
            {
                return UserErrors.ClerkUserRequired;
            }

            var now = DateTime.UtcNow;
            var user = new User
            {
                Id = Guid.NewGuid(),
                ClerkUserId = clerkUserId,
                CreatedAt = now,
                UpdatedAt = now
            };

            return user;
        }
    }
}