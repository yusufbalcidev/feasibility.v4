using feasibility.Entity.Entities.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace feasibility.DataAccess.Configurations;

public class LocationTypeMaintenanceConfiguration : IEntityTypeConfiguration<LocationTypeMaintenance>
{
    public void Configure(EntityTypeBuilder<LocationTypeMaintenance> builder)
    {
        builder.ToTable("LocationTypeMaintenances");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(t => t.Description).HasMaxLength(500);

        builder.Property(t => t.CreatedByName).HasMaxLength(150);
        builder.Property(t => t.UpdatedByName).HasMaxLength(150);
        builder.Property(t => t.DeletedByName).HasMaxLength(150);

        builder.Property(t => t.IsDeleted).HasDefaultValue(false);

        builder.HasIndex(t => t.Name);
    }
}
