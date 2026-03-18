using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Ecommerce.Domain.Common
{
    public static partial class SlugGenerator
    {
        [GeneratedRegex(@"[^a-z0-9\s-]", RegexOptions.Compiled)]
        private static partial Regex InvalidCharsRegex();

        [GeneratedRegex(@"[\s-]+", RegexOptions.Compiled)]
        private static partial Regex MultiHyphensRegex();

        public static string Generate(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;

            string normalized = value.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new();

            foreach (char c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            string slug = sb.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();

            slug = InvalidCharsRegex().Replace(slug, string.Empty);

            slug = MultiHyphensRegex().Replace(slug, "-");

            slug = slug.Trim('-');

            return slug.Length > 100 ? slug[..100].Trim('-').ToString() : slug;
        }
    }
}
