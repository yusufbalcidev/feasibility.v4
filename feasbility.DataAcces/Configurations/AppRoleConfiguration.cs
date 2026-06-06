using feasibility.Entity.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace feasibility.DataAccess.Configurations;

public class AppRoleConfiguration : IEntityTypeConfiguration<AppRole>
{
    public void Configure(EntityTypeBuilder<AppRole> builder)
    {
        builder.ToTable("Roles");

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(80);

        builder.Property(r => r.Description).HasMaxLength(500);

        builder.Property(r => r.CreatedByName).HasMaxLength(150);
        builder.Property(r => r.UpdatedByName).HasMaxLength(150);
        builder.Property(r => r.DeletedByName).HasMaxLength(150);

        builder.Property(r => r.IsDeleted).HasDefaultValue(false);
    }
}
