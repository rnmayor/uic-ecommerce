namespace Ecommerce.Application.Common.Tenancy;

/// <summary>
/// Marks an endpoint as not requiring tenant resolution.
/// Used for endpoints that operate outside of a tenant context,
/// such as onboarding, authentication callbacks, health checks,
/// and user self-discovery APIs.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class SkipTenantResolutionAttribute : Attribute { }
