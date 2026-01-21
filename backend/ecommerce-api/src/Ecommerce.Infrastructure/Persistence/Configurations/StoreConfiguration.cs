using Ecommerce.Domain.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Persistence.Configurations;

public class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.ToTable("stores");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.TenantId).IsRequired();
        builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.UpdatedAt).IsRequired();

        builder.HasIndex(s => s.TenantId);
    }
}
