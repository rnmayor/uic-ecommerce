using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Users;

public class User : Entity
{
    public string ClerkUserId { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    private User() { } // For EF
    public User(string clerkUserId)
    {
        if (string.IsNullOrWhiteSpace(clerkUserId))
        {
            throw new DomainException("Clerk user id is required.");
        }

        Id = Guid.NewGuid();
        ClerkUserId = clerkUserId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }
}
