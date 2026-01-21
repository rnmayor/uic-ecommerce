using Ecommerce.Domain.Tenants;
using Ecommerce.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence;

public class EcommerceDbContext : DbContext
{
  public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options) : base(options)
  {

  }

  public DbSet<Tenant> Tenants => Set<Tenant>();
  public DbSet<Store> Stores => Set<Store>();
  public DbSet<User> Users => Set<User>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(EcommerceDbContext).Assembly);
    base.OnModelCreating(modelBuilder);
  }
}
