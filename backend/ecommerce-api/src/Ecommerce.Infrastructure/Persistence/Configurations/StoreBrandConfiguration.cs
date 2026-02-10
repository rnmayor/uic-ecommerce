using Ecommerce.Domain.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Persistence.Configurations;

internal sealed class StoreBrandConfiguration : IEntityTypeConfiguration<StoreBrand>
{
    public void Configure(EntityTypeBuilder<StoreBrand> builder)
    {
        builder.ToTable("store_brands");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.NormalizedName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasIndex(x => x.Name).IsUnique();
        builder.HasIndex(x => x.NormalizedName).IsUnique();

        // Navigation: one-to-many with StoreInstances
        builder.HasMany(x => x.StoreInstances)
            .WithOne(x => x.StoreBrand)
            .HasForeignKey(x => x.StoreBrandId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
