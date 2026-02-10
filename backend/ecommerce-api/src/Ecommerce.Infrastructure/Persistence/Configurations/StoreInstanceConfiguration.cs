using Ecommerce.Domain.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Persistence.Configurations;

internal sealed class StoreInstanceConfiguration : IEntityTypeConfiguration<StoreInstance>
{
    public void Configure(EntityTypeBuilder<StoreInstance> builder)
    {
        builder.ToTable("store_instances");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.TenantId).IsRequired();
        builder.Property(s => s.StoreBrandId).IsRequired();
        builder.Property(s => s.DisplayName).IsRequired().HasMaxLength(200);
        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.UpdatedAt).IsRequired();

        builder.HasIndex(s => s.TenantId);
        builder.HasIndex(s => s.StoreBrandId);
        // Store display name must be unique per tenant
        builder.HasIndex(s => new { s.TenantId, s.DisplayName }).IsUnique();

        // Navigation: many-to-one with StoreBrand
        builder.HasOne(x => x.StoreBrand)
            .WithMany(x => x.StoreInstances)
            .HasForeignKey(x => x.StoreBrandId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
