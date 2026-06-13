namespace Ecommerce.Application.Common.Interfaces;

public interface ITenantMemberAuthorizationService
{
    Task<bool> IsTenantMemberAsync(
        Guid tenantId,
        Guid userId,
        string[] allowedRoles,
        CancellationToken cancellationToken = default
    );
}
