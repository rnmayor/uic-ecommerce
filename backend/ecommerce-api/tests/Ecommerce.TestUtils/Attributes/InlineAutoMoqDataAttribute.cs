using AutoFixture.Xunit2;
using Xunit;

namespace Ecommerce.TestUtils.Attributes;

public sealed class InlineAutoMoqDataAttribute : CompositeDataAttribute
{
    public InlineAutoMoqDataAttribute(params object[] values)
        : base(new InlineDataAttribute(values), new AutoMoqDataAttribute())
    { }
}
