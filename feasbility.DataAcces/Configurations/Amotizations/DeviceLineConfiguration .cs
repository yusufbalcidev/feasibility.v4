using feasibility.Entity.Entities.FeasibilityAmortization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace feasibility.DataAccess.Configurations.Amotizations
{
    public class DeviceLineConfiguration : IEntityTypeConfiguration<DeviceLine>
    {
        public void Configure(EntityTypeBuilder<DeviceLine> builder)
        {
            builder.ToTable("FeasibilityDeviceLines");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.DeviceType).IsRequired();
            builder.Property(d => d.DeviceCount).IsRequired();
            builder.Property(d => d.SocketCount).IsRequired();

            builder.Property(d => d.DailyChargesPerSocket).HasColumnType("decimal(5,2)");

            // Satış fiyatı — kullanıcı TL girer, $/€ sadece UI'da gösterilir
            builder.Property(d => d.SalePriceTl).HasColumnType("decimal(18,2)");

            // Alış (elektrik) fiyatı — kullanıcı TL girer, $/€ sadece UI'da gösterilir
            builder.Property(d => d.PurchasePriceTl).HasColumnType("decimal(18,2)");

            // Lokasyon bedeli — orijinal değer + para birimi + TL shadow
            builder.Property(d => d.UnitLocationCost).HasColumnType("decimal(18,2)");
            builder.Property(d => d.UnitLocationCostCurrency).IsRequired();
            builder.Property(d => d.UnitLocationCostTl).HasColumnType("decimal(18,2)");

            builder.Property(d => d.AgreementGenre).IsRequired();
            builder.Property(d => d.AgreementRate).HasColumnType("decimal(5,2)");

            // Audit
            builder.Property(d => d.CreatedByName).HasMaxLength(150);
            builder.Property(d => d.UpdatedByName).HasMaxLength(150);
            builder.Property(d => d.DeletedByName).HasMaxLength(150);
            builder.Property(d => d.IsDeleted).HasDefaultValue(false);

            // FK: Study silinirse DeviceLine'lar da silinir
            builder.HasOne(d => d.Study)
                .WithMany(s => s.DeviceLines)
                .HasForeignKey(d => d.StudyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Bir study içinde her istasyon türünden yalnızca bir satır olabilir
            builder.HasIndex(d => new { d.StudyId, d.DeviceType }).IsUnique();
        }
    }
}
