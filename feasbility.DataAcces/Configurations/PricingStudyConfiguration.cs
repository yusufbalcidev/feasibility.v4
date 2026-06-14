using feasibility.Entity.Entities.FeasibilityPricing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace feasibility.DataAccess.Configurations;

public class PricingStudyConfiguration : IEntityTypeConfiguration<PricingStudy>
{
    public void Configure(EntityTypeBuilder<PricingStudy> builder)
    {
        builder.ToTable("PricingStudies");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();

        builder.Property(s => s.FeasibilityName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Kind).IsRequired();

        builder.Property(s => s.UsdRate).HasColumnType("decimal(18,4)");
        builder.Property(s => s.EurRate).HasColumnType("decimal(18,4)");

        builder.Property(s => s.InflationTl).HasColumnType("decimal(8,4)");
        builder.Property(s => s.InflationUsd).HasColumnType("decimal(8,4)");
        builder.Property(s => s.InflationEur).HasColumnType("decimal(8,4)");

        builder.Property(s => s.VatRate).HasColumnType("decimal(8,4)").HasDefaultValue(20m);
        builder.Property(s => s.CommissionRate).HasColumnType("decimal(8,4)").HasDefaultValue(2.7m);

        builder.Property(s => s.CreatedByName).HasMaxLength(150);
        builder.Property(s => s.UpdatedByName).HasMaxLength(150);
        builder.Property(s => s.DeletedByName).HasMaxLength(150);

        builder.Property(s => s.IsDeleted).HasDefaultValue(false);
    }
}
