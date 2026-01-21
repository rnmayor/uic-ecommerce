using Ecommerce.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
  public void Configure(EntityTypeBuilder<User> builder)
  {
    builder.ToTable("users");
    builder.HasKey(u => u.Id);

    builder.Property(u => u.ClerkUserId).IsRequired().HasMaxLength(100);
    builder.Property(u => u.CreatedAt).IsRequired();
    builder.Property(u => u.UpdatedAt).IsRequired();

    builder.HasIndex(u => u.ClerkUserId).IsUnique();
  }
}
