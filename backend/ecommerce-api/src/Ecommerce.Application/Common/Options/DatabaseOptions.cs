using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.Common.Options;

public sealed class DatabaseOptions
{
    [Required]
    public string ConnectionString { get; set; } = default!;
}
