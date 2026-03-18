namespace Ecommerce.Domain.Common
{
    public static class IdentityNormalizer
    {
        public static string Normalize(string value) => value.Trim().ToUpperInvariant();
    }
}
