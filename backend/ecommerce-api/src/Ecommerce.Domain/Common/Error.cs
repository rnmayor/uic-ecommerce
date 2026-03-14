using System.Net;

namespace Ecommerce.Domain.Common
{
    public sealed record Error(string Code, string Description, HttpStatusCode StatusCode)
    {
        public static readonly Error None = new(string.Empty, string.Empty, HttpStatusCode.OK);
    }
}
