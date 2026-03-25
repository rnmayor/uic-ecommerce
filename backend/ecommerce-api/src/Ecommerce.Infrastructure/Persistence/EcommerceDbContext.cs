using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Stores;
using Ecommerce.Domain.Tenants;
using Ecommerce.Domain.Users;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Ecommerce.Infrastructure.Persistence
{
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
                        .GetMethod(nameof(SetTenantFilter), BindingFlags.NonPublic | BindingFlags.Instance)!
                        .MakeGenericMethod(entityType.ClrType);

                    method.Invoke(this, new object[] { modelBuilder });
                }
            }
        }

        private void SetTenantFilter<TEntity>(
            ModelBuilder modelBuilder
        ) where TEntity : TenantEntity
        {
            modelBuilder.Entity<TEntity>().HasQueryFilter(e => !_tenantContext.IsResolved || e.TenantId == _tenantContext.TenantId);
        }
    }
}