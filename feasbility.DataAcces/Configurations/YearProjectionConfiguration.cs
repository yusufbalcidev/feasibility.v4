using feasibility.Entity.Entities.FeasibilityAmortization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace feasibility.DataAccess.Configurations;

public class YearProjectionConfiguration : IEntityTypeConfiguration<YearProjection>
{
    public void Configure(EntityTypeBuilder<YearProjection> builder)
    {
        builder.ToTable("YearProjections");

        builder.HasKey(y => y.Id);

        builder.Property(y => y.DailyChargePerSocket).HasColumnType("decimal(18,4)");
        builder.Property(y => y.SalePriceH1).HasColumnType("decimal(18,2)");
        builder.Property(y => y.SalePriceH2).HasColumnType("decimal(18,2)");
        builder.Property(y => y.PurchasePriceH1).HasColumnType("decimal(18,2)");
        builder.Property(y => y.PurchasePriceH2).HasColumnType("decimal(18,2)");
        builder.Property(y => y.UsdRate).HasColumnType("decimal(18,2)");

        builder.Property(y => y.CreatedByName).HasMaxLength(150);
        builder.Property(y => y.UpdatedByName).HasMaxLength(150);
        builder.Property(y => y.DeletedByName).HasMaxLength(150);

        builder.Property(y => y.IsDeleted).HasDefaultValue(false);

        builder.HasOne(y => y.DeviceLine)
            .WithMany(d => d.YearProjections)
            .HasForeignKey(y => y.DeviceLineId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(y => y.DeviceLineId);
    }
}
