using feasibility.Entity.Entities.FeasibilityPricing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace feasibility.DataAccess.Configurations;

public class PricingStationConfiguration : IEntityTypeConfiguration<PricingStation>
{
    public void Configure(EntityTypeBuilder<PricingStation> builder)
    {
        builder.ToTable("PricingStations");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();

        builder.Property(s => s.DeviceType).IsRequired();
        builder.Property(s => s.SocketCount).IsRequired();
        builder.Property(s => s.EconomicLifeYears).IsRequired();

        builder.Property(s => s.DailyKwhPerSocket).HasColumnType("decimal(18,4)");
        builder.Property(s => s.DiscountRate).HasColumnType("decimal(8,4)");
        builder.Property(s => s.TargetProfitMargin).HasColumnType("decimal(8,4)").HasDefaultValue(10m);

        builder.Property(s => s.HardwareCost).HasColumnType("decimal(18,4)");
        builder.Property(s => s.HardwareCostCurrency).IsRequired();
        builder.Property(s => s.HardwareCostTl).HasColumnType("decimal(18,4)");

        builder.Property(s => s.InfrastructureCost).HasColumnType("decimal(18,4)");
        builder.Property(s => s.InfrastructureCostCurrency).IsRequired();
        builder.Property(s => s.InfrastructureCostTl).HasColumnType("decimal(18,4)");

        builder.Property(s => s.AnnualOpex).HasColumnType("decimal(18,4)");
        builder.Property(s => s.AnnualOpexCurrency).IsRequired();
        builder.Property(s => s.AnnualOpexUsd).HasColumnType("decimal(18,4)");

        builder.Property(s => s.GridElectricityCost).HasColumnType("decimal(18,6)");
        builder.Property(s => s.GridElectricityCostLow).HasColumnType("decimal(18,6)");
        builder.Property(s => s.GridElectricityCostHigh).HasColumnType("decimal(18,6)");

        builder.Property(s => s.CreatedByName).HasMaxLength(150);
        builder.Property(s => s.UpdatedByName).HasMaxLength(150);
        builder.Property(s => s.DeletedByName).HasMaxLength(150);

        builder.Property(s => s.IsDeleted).HasDefaultValue(false);

        builder.HasOne(s => s.PricingStudy)
            .WithMany(p => p.Stations)
            .HasForeignKey(s => s.PricingStudyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => s.PricingStudyId);
    }
}
