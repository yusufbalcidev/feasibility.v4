using feasibility.Entity.Entities.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace feasibility.DataAccess.Configurations;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions");

        builder.HasKey(rp => rp.Id);

        builder.Property(rp => rp.CreatedByName).HasMaxLength(150);
        builder.Property(rp => rp.UpdatedByName).HasMaxLength(150);
        builder.Property(rp => rp.DeletedByName).HasMaxLength(150);

        builder.HasOne(rp => rp.Role)
            .WithMany()
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rp => rp.Page)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(rp => new { rp.RoleId, rp.PageId }).IsUnique();
    }
}
