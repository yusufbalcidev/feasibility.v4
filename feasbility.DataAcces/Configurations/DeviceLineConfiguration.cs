using feasibility.Entity.Entities.FeasibilityAmortization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace feasibility.DataAccess.Configurations;

public class DeviceLineConfiguration : IEntityTypeConfiguration<DeviceLine>
{
    public void Configure(EntityTypeBuilder<DeviceLine> builder)
    {
        builder.ToTable("DeviceLines");

        builder.HasKey(d => d.Id);
        // PK uygulama tarafında üretilir (Guid.NewGuid). ValueGeneratedOnAdd olursa,
        // dolu Guid ile eklenen yeni satırlar EF tarafından Modified sanılıp "0 row affected" hatası verir.
        builder.Property(d => d.Id).ValueGeneratedNever();

        builder.Property(d => d.DeviceType).IsRequired();
        builder.Property(d => d.DeviceCount).IsRequired();
        builder.Property(d => d.SocketCount).IsRequired();
        builder.Property(d => d.AgreementGenre).IsRequired();
        builder.Property(d => d.UnitLocationCostCurrency).IsRequired();

        builder.Property(d => d.DailyChargesPerSocket).HasColumnType("decimal(18,4)");
        builder.Property(d => d.AvgKwh).HasColumnType("decimal(18,4)");
        builder.Property(d => d.SalePriceTl).HasColumnType("decimal(18,4)");
        builder.Property(d => d.PurchasePriceTl).HasColumnType("decimal(18,4)");
        builder.Property(d => d.UnitLocationCost).HasColumnType("decimal(18,4)");
        builder.Property(d => d.UnitLocationCostTl).HasColumnType("decimal(18,4)");
        builder.Property(d => d.AgreementRate).HasColumnType("decimal(8,4)");

        builder.Property(d => d.CreatedByName).HasMaxLength(150);
        builder.Property(d => d.UpdatedByName).HasMaxLength(150);
        builder.Property(d => d.DeletedByName).HasMaxLength(150);

        builder.Property(d => d.IsDeleted).HasDefaultValue(false);

        builder.HasOne(d => d.Study)
            .WithMany(s => s.DeviceLines)
            .HasForeignKey(d => d.StudyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(d => d.StudyId);
    }
}
