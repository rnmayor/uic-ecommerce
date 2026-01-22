using Ecommerce.Domain.Tenants;
using Ecommerce.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Persistence.Configurations;

internal sealed class TenantUserConfiguration : IEntityTypeConfiguration<TenantUser>
{
    public void Configure(EntityTypeBuilder<TenantUser> builder)
    {
        builder.ToTable("tenant_users");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Role).IsRequired().HasMaxLength(50);
        builder.Property(x => x.CreatedAt).IsRequired();

        // Composite uniqueness: user can belong to tenant only once
        builder.HasIndex(x => new { x.TenantId, x.UserId }).IsUnique();

        // FK relationships (no navigation required yet)
        builder.HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
