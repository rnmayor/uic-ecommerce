using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace Ecommerce.TestUtils.Attributes;

public sealed class AutoMoqDataAttribute : AutoDataAttribute
{
    public AutoMoqDataAttribute() : base(() =>
    {
        var fixture = new Fixture();

        fixture.Customize(new AutoMoqCustomization
        {
            ConfigureMembers = true
        });

        return fixture;
    })
    { }
}
