using feasibility.Entity.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace feasibility.DataAccess.Configurations;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("Users");

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.CreatedByName).HasMaxLength(150);
        builder.Property(u => u.UpdatedByName).HasMaxLength(150);
        builder.Property(u => u.DeletedByName).HasMaxLength(150);

        builder.Property(u => u.IsDeleted).HasDefaultValue(false);

        builder.HasQueryFilter(u => !u.IsDeleted);
    }
}
