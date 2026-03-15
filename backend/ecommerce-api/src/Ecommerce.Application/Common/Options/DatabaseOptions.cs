using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.Common.Options
{
    public sealed class DatabaseOptions
    {
        public const string SectionName = "DatabaseOptions";

        [Required]
        public string ConnectionString { get; set; } = default!;
    }
}