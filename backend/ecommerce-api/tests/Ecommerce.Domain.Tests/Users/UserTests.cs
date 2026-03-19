using Ecommerce.Domain.Users;

namespace Ecommerce.Domain.Tests.Users
{
    public sealed class UserTests
    {
        [Fact]
        public void CreatesUser_WhenValid()
        {
            var clerkUserId = "123";
            var user = User.Create(clerkUserId);

            Assert.True(user.IsSuccess);
            Assert.NotEqual(Guid.Empty, user.Value.Id);
            Assert.Equal(clerkUserId, user.Value.ClerkUserId);
            Assert.Equal(user.Value.CreatedAt, user.Value.UpdatedAt);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void ReturnsFailure_WhenClerkUserIdIsNullOrWhitespace(string clerkUserId)
        {
            var user = User.Create(clerkUserId);

            Assert.True(user.IsFailure);
            Assert.Equal(UserErrors.ClerkUserRequired, user.Error);
        }

        [Fact]
        public void SetsCreatedAtToUtcNow()
        {
            var clerkUserId = "123";

            var before = DateTime.UtcNow;
            var user = User.Create(clerkUserId);
            var after = DateTime.UtcNow;

            Assert.True(user.IsSuccess);
            Assert.InRange(user.Value.CreatedAt, before, after);
        }
    }
}