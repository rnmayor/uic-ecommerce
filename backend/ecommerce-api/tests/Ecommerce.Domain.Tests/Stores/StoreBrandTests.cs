using Ecommerce.Domain.Stores;

namespace Ecommerce.Domain.Tests.Stores
{
    public sealed class StoreBrandTests
    {
        [Fact]
        public void CreateStoreBrand_WhenValid()
        {
            var name = "My Brand";
            var storeBrand = StoreBrand.Create(name);

            Assert.True(storeBrand.IsSuccess);
            Assert.NotEqual(Guid.Empty, storeBrand.Value.Id);
            Assert.Equal(name, storeBrand.Value.Name);
            Assert.Equal(StoreBrand.Normalize(name), storeBrand.Value.NormalizedName);
            Assert.Equal(storeBrand.Value.CreatedAt, storeBrand.Value.UpdatedAt);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void ReturnsFailure_WhenNameIsNullOrWhitespace(string name)
        {
            var storeBrand = StoreBrand.Create(name);

            Assert.True(storeBrand.IsFailure);
            Assert.Equal(StoreBrandErrors.NameRequired, storeBrand.Error);
        }

        [Fact]
        public void TrimsName_OnCreation()
        {
            var name = "   My Brand   ";
            var storeBrand = StoreBrand.Create(name);

            Assert.True(storeBrand.IsSuccess);
            Assert.Equal("My Brand", storeBrand.Value.Name);
        }

        [Fact]
        public void SetsCreatedAtToUtcNow()
        {
            var name = "My Brand";

            var before = DateTime.UtcNow;
            var storeBrand = StoreBrand.Create(name);
            var after = DateTime.UtcNow;

            Assert.True(storeBrand.IsSuccess);
            Assert.InRange(storeBrand.Value.CreatedAt, before, after);
        }
    }
}
