namespace Ecommerce.Application.Common.Authorization;

public interface ITenantAuthorizationService
{
    Task<bool> IsTenantMemberAsync(
      Guid tenantId,
      Guid userId,
      string[] allowedRoles,
      CancellationToken cancellationToken = default
    );
}
