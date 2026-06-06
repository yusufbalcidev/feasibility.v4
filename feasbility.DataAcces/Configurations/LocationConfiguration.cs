using feasibility.Entity.Entities.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace feasibility.DataAccess.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("Locations");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(l => l.Description).HasMaxLength(500);

        builder.Property(l => l.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(l => l.District)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(l => l.Address).HasMaxLength(300);

        builder.Property(l => l.Latitude).HasColumnType("decimal(9,6)");
        builder.Property(l => l.Longitude).HasColumnType("decimal(9,6)");

        builder.Property(l => l.CreatedByName).HasMaxLength(150);
        builder.Property(l => l.UpdatedByName).HasMaxLength(150);
        builder.Property(l => l.DeletedByName).HasMaxLength(150);

        builder.Property(l => l.IsDeleted).HasDefaultValue(false);

        builder.HasOne(l => l.LocationTypeMaintenance)
            .WithMany(t => t.Locations)
            .HasForeignKey(l => l.LocationTypeMaintenanceId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
