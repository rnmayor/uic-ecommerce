using Ecommerce.Domain.Common;
using Ecommerce.Domain.Stores;

namespace Ecommerce.Domain.Tests.Stores;

public sealed class StoreBrandTests
{
    [Fact]
    public void CreateStoreBrand_WhenValid()
    {
        var name = "My Brand";
        var storeBrand = new StoreBrand(name);

        Assert.NotEqual(Guid.Empty, storeBrand.Id);
        Assert.Equal(name, storeBrand.Name);
        Assert.Equal(name.Trim().ToLowerInvariant(), storeBrand.NormalizedName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Throws_WhenNameIsNullOrWhitespace(string name)
    {
        var ex = Assert.Throws<DomainException>(() =>
            new StoreBrand(name));

        Assert.Contains("Store brand name is required", ex.Message);
    }

    [Fact]
    public void TrimsName_OnCreation()
    {
        var name = "My Brand";
        var storeBrand = new StoreBrand(name);

        Assert.Equal("My Brand", storeBrand.Name);
    }

    [Fact]
    public void SetsCreatedAtToUtcNow()
    {
        var name = "My Brand";

        var before = DateTime.UtcNow;
        var storeBrand = new StoreBrand(name);
        var after = DateTime.UtcNow;

        Assert.InRange(storeBrand.CreatedAt, before, after);
    }
}
