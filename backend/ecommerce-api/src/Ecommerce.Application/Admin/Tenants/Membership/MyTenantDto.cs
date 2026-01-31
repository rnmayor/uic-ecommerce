namespace Ecommerce.Application.Admin.Tenants.Membership;

public sealed class MyTenantDto
{
    public Guid TenantId { get; init; }
    public string Name { get; init; } = default!;
    public bool IsOwner { get; init; }
}
