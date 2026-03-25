using Ecommerce.Domain.Common;

namespace Ecommerce.Application.Common.Validation
{
    public static class SlugValidator
    {
        public static bool IsCanonical(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) return false;

            var normalized = SlugGenerator.Generate(slug);

            return string.Equals(slug, normalized, StringComparison.Ordinal);
        }
    }
}
