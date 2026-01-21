using Ecommerce.Domain.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
  public void Configure(EntityTypeBuilder<Tenant> builder)
  {
    builder.ToTable("tenants");
    builder.HasKey(t => t.Id);

    builder.Property(t => t.Name).IsRequired().HasMaxLength(200);
    builder.Property(t => t.OwnerUserId).IsRequired().HasMaxLength(100);
    builder.Property(t => t.CreatedAt).IsRequired();
    builder.Property(t => t.UpdatedAt).IsRequired();

    builder.HasIndex(t => t.OwnerUserId);
  }
}
