using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Stores;

public class StoreBrand : Entity
{
    public string Name { get; private set; } = default!;
    public string NormalizedName { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }

    private readonly List<StoreInstance> _storeInstances = new();
    public IReadOnlyCollection<StoreInstance> StoreInstances => _storeInstances.AsReadOnly();

    private StoreBrand() { } // EF

    public StoreBrand(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Store brand name is required.");
        }

        Id = Guid.NewGuid();
        Name = name.Trim();
        NormalizedName = Normalize(name);
        CreatedAt = DateTime.UtcNow;
    }

    private static string Normalize(string name)
    {
        return name.Trim().ToLowerInvariant();
    }
}
