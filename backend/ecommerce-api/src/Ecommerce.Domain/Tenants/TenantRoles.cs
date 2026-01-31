namespace Ecommerce.Domain.Tenants;

public static class TenantRoles
{
    public const string Owner = "Owner";
    public const string Admin = "Admin";
    public const string Manager = "Manager";
    public const string Staff = "Staff";
    public const string Customer = "Customer";

    public static IReadOnlySet<string> All { get; } = new HashSet<string>
    {
        Owner, Admin, Manager, Staff, Customer
    };
}
