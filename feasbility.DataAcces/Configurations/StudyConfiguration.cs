using feasibility.Entity.Entities.FeasibilityAmortization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace feasibility.DataAccess.Configurations;

public class StudyConfiguration : IEntityTypeConfiguration<Study>
{
    public void Configure(EntityTypeBuilder<Study> builder)
    {
        builder.ToTable("Studies");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.FeasibilityName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Kind).IsRequired();

        builder.Property(s => s.UsdRate).HasColumnType("decimal(18,4)");
        builder.Property(s => s.EurRate).HasColumnType("decimal(18,4)");

        builder.Property(s => s.InflationTl).HasColumnType("decimal(8,4)");
        builder.Property(s => s.InflationUsd).HasColumnType("decimal(8,4)");
        builder.Property(s => s.InflationEur).HasColumnType("decimal(8,4)");

        builder.Property(s => s.MonthlyRent).HasColumnType("decimal(18,4)");
        builder.Property(s => s.RentCurrency).IsRequired();
        builder.Property(s => s.MonthlyRentTl).HasColumnType("decimal(18,4)");

        builder.Property(s => s.PostWarrantyMaintenanceCost).HasColumnType("decimal(18,4)");
        builder.Property(s => s.PostWarrantyMaintenanceCurrency).IsRequired();
        builder.Property(s => s.PostWarrantyMaintenanceCostTl).HasColumnType("decimal(18,4)");

        builder.Property(s => s.AdvertisingRevenue).HasColumnType("decimal(18,4)");
        builder.Property(s => s.AdvertisingRevenueCurrency).IsRequired();
        builder.Property(s => s.AdvertisingRevenueTl).HasColumnType("decimal(18,4)");

        builder.Property(s => s.StationUnitCost).HasColumnType("decimal(18,4)");
        builder.Property(s => s.StationUnitCostCurrency).IsRequired();
        builder.Property(s => s.StationUnitCostTl).HasColumnType("decimal(18,4)");

        builder.Property(s => s.ProviderEntryFee).HasColumnType("decimal(18,4)");
        builder.Property(s => s.ProviderEntryFeeCurrency).IsRequired();
        builder.Property(s => s.ProviderEntryFeeTl).HasColumnType("decimal(18,4)");

        builder.Property(s => s.InfrastructureCost).HasColumnType("decimal(18,4)");
        builder.Property(s => s.InfrastructureCostCurrency).IsRequired();
        builder.Property(s => s.InfrastructureCostTl).HasColumnType("decimal(18,4)");

        builder.Property(s => s.DeviceUnitCost).HasColumnType("decimal(18,4)");
        builder.Property(s => s.DeviceUnitCostCurrency).IsRequired();
        builder.Property(s => s.DeviceUnitCostTl).HasColumnType("decimal(18,4)");

        builder.Property(s => s.LoanAmount).HasColumnType("decimal(18,4)");
        builder.Property(s => s.LoanAnnualInterestRate).HasColumnType("decimal(8,4)");

        builder.Property(s => s.CreatedByName).HasMaxLength(150);
        builder.Property(s => s.UpdatedByName).HasMaxLength(150);
        builder.Property(s => s.DeletedByName).HasMaxLength(150);

        builder.Property(s => s.IsDeleted).HasDefaultValue(false);

        builder.HasOne(s => s.Location)
            .WithMany()
            .HasForeignKey(s => s.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => s.LocationId);
    }
}
