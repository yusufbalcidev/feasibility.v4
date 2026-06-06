using feasibility.Entity.Entities.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace feasibility.DataAccess.Configurations;

public class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
{
    public void Configure(EntityTypeBuilder<ActivityLog> builder)
    {
        builder.ToTable("ActivityLogs");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.UserName).HasMaxLength(150);
        builder.Property(l => l.UserEmail).HasMaxLength(256);
        builder.Property(l => l.HttpMethod).IsRequired().HasMaxLength(10);
        builder.Property(l => l.Path).IsRequired().HasMaxLength(400);
        builder.Property(l => l.QueryString).HasMaxLength(1000);
        builder.Property(l => l.Controller).HasMaxLength(150);
        builder.Property(l => l.Action).HasMaxLength(150);
        builder.Property(l => l.IpAddress).HasMaxLength(60);
        builder.Property(l => l.UserAgent).HasMaxLength(500);
        builder.Property(l => l.ActionType).IsRequired().HasMaxLength(50);
        builder.Property(l => l.Description).HasMaxLength(1000);
        builder.Property(l => l.ErrorMessage).HasMaxLength(2000);
        builder.Property(l => l.RequestContentType).HasMaxLength(150);
        builder.Property(l => l.ResponseContentType).HasMaxLength(150);
        builder.Property(l => l.RequestBody).HasColumnType("nvarchar(max)");
        builder.Property(l => l.ResponseBody).HasColumnType("nvarchar(max)");

        builder.Property(l => l.CreatedByName).HasMaxLength(150);
        builder.Property(l => l.UpdatedByName).HasMaxLength(150);
        builder.Property(l => l.DeletedByName).HasMaxLength(150);

        builder.HasIndex(l => l.CreatedAt);
        builder.HasIndex(l => l.UserId);
    }
}
