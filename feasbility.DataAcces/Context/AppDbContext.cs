using feasibility.DataAccess.Seeds;
using feasibility.Entity.Entities.Authorization;
using feasibility.Entity.Entities.Common;
using feasibility.Entity.Entities.FeasibilityAmortization;
using feasibility.Entity.Entities.FeasibilityPricing;
using feasibility.Entity.Entities.Identity;
using feasibility.Entity.Entities.Locations;
using feasibility.Entity.Entities.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace feasibility.DataAccess.Context;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor)
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<Location> Locations => Set<Location>();
    public DbSet<LocationTypeMaintenance> LocationTypeMaintenances => Set<LocationTypeMaintenance>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<Page> Pages => Set<Page>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<Study> Studies => Set<Study>();
    public DbSet<DeviceLine> DeviceLines => Set<DeviceLine>();
    public DbSet<YearProjection> YearProjections => Set<YearProjection>();
    public DbSet<PricingStudy> PricingStudies => Set<PricingStudy>();
    public DbSet<PricingStation> PricingStations => Set<PricingStation>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<Guid>>().ToTable("UserTokens");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<Guid>>().ToTable("RoleClaims");

        ApplySoftDeleteFilters(builder);
        SeedStaticData(builder);
    }

    private static void ApplySoftDeleteFilters(ModelBuilder builder)
    {
        builder.Entity<Location>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<LocationTypeMaintenance>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<ActivityLog>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Page>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<RolePermission>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Study>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<DeviceLine>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<YearProjection>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<PricingStudy>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<PricingStation>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<AppUser>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<AppRole>().HasQueryFilter(e => !e.IsDeleted);
    }

    private static void SeedStaticData(ModelBuilder builder)
    {
        builder.Entity<AppRole>().HasData(RoleSeed.All());
        builder.Entity<AppUser>().HasData(UserSeed.All());
        builder.Entity<IdentityUserRole<Guid>>().HasData(UserSeed.SuperAdminAssignment());
        builder.Entity<Page>().HasData(PageSeed.All());
        builder.Entity<RolePermission>().HasData(RolePermissionSeed.All());
    }

    public override int SaveChanges()
    {
        ApplyAuditAndSoftDelete();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditAndSoftDelete();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditAndSoftDelete()
    {
        var (userId, userName) = GetCurrentUser();
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.CreatedByName = userName;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedBy = userId;
                    entry.Entity.UpdatedByName = userName;
                    if (entry.Entity.IsDeleted && entry.Property(nameof(BaseEntity.IsDeleted)).IsModified)
                    {
                        entry.Entity.DeletedAt = now;
                        entry.Entity.DeletedBy = userId;
                        entry.Entity.DeletedByName = userName;
                    }
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = now;
                    entry.Entity.DeletedBy = userId;
                    entry.Entity.DeletedByName = userName;
                    break;
            }
        }

        foreach (var entry in ChangeTracker.Entries<AppUser>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.CreatedByName = userName;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedBy = userId;
                    entry.Entity.UpdatedByName = userName;
                    if (entry.Entity.IsDeleted && entry.Property(nameof(AppUser.IsDeleted)).IsModified)
                    {
                        entry.Entity.DeletedAt = now;
                        entry.Entity.DeletedBy = userId;
                        entry.Entity.DeletedByName = userName;
                    }
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = now;
                    entry.Entity.DeletedBy = userId;
                    entry.Entity.DeletedByName = userName;
                    break;
            }
        }

        foreach (var entry in ChangeTracker.Entries<AppRole>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.CreatedByName = userName;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedBy = userId;
                    entry.Entity.UpdatedByName = userName;
                    if (entry.Entity.IsDeleted && entry.Property(nameof(AppRole.IsDeleted)).IsModified)
                    {
                        entry.Entity.DeletedAt = now;
                        entry.Entity.DeletedBy = userId;
                        entry.Entity.DeletedByName = userName;
                    }
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = now;
                    entry.Entity.DeletedBy = userId;
                    entry.Entity.DeletedByName = userName;
                    break;
            }
        }
    }

    private (Guid? userId, string? userName) GetCurrentUser()
    {
        var principal = _httpContextAccessor?.HttpContext?.User;
        if (principal == null || principal.Identity?.IsAuthenticated != true)
            return (null, null);

        var idValue = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        Guid? id = Guid.TryParse(idValue, out var parsed) ? parsed : null;
        var name = principal.Identity?.Name;
        return (id, name);
    }
}
