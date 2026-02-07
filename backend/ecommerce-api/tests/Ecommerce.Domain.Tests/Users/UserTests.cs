using Ecommerce.Domain.Common;
using Ecommerce.Domain.Users;

namespace Ecommerce.Domain.Tests.Users;

public sealed class UserTests
{
    [Fact]
    public void CreatesUser_WhenValid()
    {
        var clerkUserId = "123";
        var user = new User(clerkUserId);

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal(clerkUserId, user.ClerkUserId);
        Assert.Equal(user.CreatedAt, user.UpdatedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Throws_WhenClerkUserIdIsNullOrWhitespace(string clerkUserId)
    {
        var ex = Assert.Throws<DomainException>(() =>
            new User(clerkUserId));

        Assert.Contains("Clerk user id is required", ex.Message);
    }

    [Fact]
    public void SetsCreatedAtToUtcNow()
    {
        var clerkUserId = "123";

        var before = DateTime.UtcNow;
        var user = new User(clerkUserId);
        var after = DateTime.UtcNow;

        Assert.InRange(user.CreatedAt, before, after);
    }
}
