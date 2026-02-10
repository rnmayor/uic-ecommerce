using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Stores;
using Ecommerce.Domain.Tenants;
using Ecommerce.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence;

public class EcommerceDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;
    public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options, ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<StoreBrand> StoreBrands => Set<StoreBrand>();
    public DbSet<StoreInstance> StoreInstances => Set<StoreInstance>();
    public DbSet<User> Users => Set<User>();
    public DbSet<TenantUser> TenantUsers => Set<TenantUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ApplyTenantFilters(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EcommerceDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    private void ApplyTenantFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(TenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(EcommerceDbContext)
                    .GetMethod(nameof(SetTenantFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                    .MakeGenericMethod(entityType.ClrType);

                method.Invoke(null, new object[] { modelBuilder, _tenantContext });
            }
        }
    }

    private static void SetTenantFilter<TEntity>(
        ModelBuilder modelBuilder,
        ITenantContext tenantContext
    ) where TEntity : TenantEntity
    {
        if (tenantContext.IsResolved)
        {
            modelBuilder.Entity<TEntity>().HasQueryFilter(e => e.TenantId == tenantContext.TenantId);
        }
    }
}
