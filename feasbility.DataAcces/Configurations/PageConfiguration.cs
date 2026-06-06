using feasibility.Entity.Entities.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace feasibility.DataAccess.Configurations;

public class PageConfiguration : IEntityTypeConfiguration<Page>
{
    public void Configure(EntityTypeBuilder<Page> builder)
    {
        builder.ToTable("Pages");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Key)
            .IsRequired()
            .HasMaxLength(80);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.Description).HasMaxLength(500);
        builder.Property(p => p.Icon).HasMaxLength(60);

        builder.Property(p => p.CreatedByName).HasMaxLength(150);
        builder.Property(p => p.UpdatedByName).HasMaxLength(150);
        builder.Property(p => p.DeletedByName).HasMaxLength(150);

        builder.HasIndex(p => p.Key).IsUnique();
    }
}
