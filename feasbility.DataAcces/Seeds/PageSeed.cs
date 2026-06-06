using feasibility.Entity.Entities.Authorization;
namespace feasibility.DataAccess.Seeds;
public static class PageSeed
{
    public static readonly Guid DashboardPageId = new("11111111-1111-1111-1111-000000000001");
    public static readonly Guid LocationPageId = new("11111111-1111-1111-1111-000000000002");
    public static readonly Guid LocationTypeMaintenancePageId = new("11111111-1111-1111-1111-000000000003");
    public static readonly Guid UserPageId = new("11111111-1111-1111-1111-000000000004");
    public static readonly Guid RolePageId = new("11111111-1111-1111-1111-000000000005");
    public static readonly Guid ActivityLogPageId = new("11111111-1111-1111-1111-000000000006");
    public static readonly Guid FeasibilityPageId = new("11111111-1111-1111-1111-000000000007");
    public const string DashboardKey = "Dashboard";
    public const string LocationKey = "Location";
    public const string LocationTypeMaintenanceKey = "LocationTypeMaintenance";
    public const string UserKey = "User";
    public const string RoleKey = "Role";
    public const string ActivityLogKey = "ActivityLog";
    public const string FeasibilityKey = "Feasibility";
    public static IReadOnlyList<Page> All() => new List<Page>
    {
        new() { Id = DashboardPageId, Key = DashboardKey, Name = "Dashboard", Icon = "dashboard", DisplayOrder = 1, CreatedAt = SeedDates.Anchor },
        new() { Id = LocationPageId, Key = LocationKey, Name = "Lokasyonlar", Icon = "location_on", DisplayOrder = 2, CreatedAt = SeedDates.Anchor },
        new() { Id = LocationTypeMaintenancePageId, Key = LocationTypeMaintenanceKey, Name = "Lokasyon Bakım Tipleri", Icon = "category", DisplayOrder = 3, CreatedAt = SeedDates.Anchor },
        new() { Id = UserPageId, Key = UserKey, Name = "Kullanıcılar", Icon = "group", DisplayOrder = 4, CreatedAt = SeedDates.Anchor },
        new() { Id = RolePageId, Key = RoleKey, Name = "Roller", Icon = "admin_panel_settings", DisplayOrder = 5, CreatedAt = SeedDates.Anchor },
        new() { Id = ActivityLogPageId, Key = ActivityLogKey, Name = "Aktivite Logları", Icon = "history", DisplayOrder = 6, CreatedAt = SeedDates.Anchor },
        new() { Id = FeasibilityPageId, Key = FeasibilityKey, Name = "Fizibilite Projeleri", Icon = "ev_station", DisplayOrder = 7, CreatedAt = SeedDates.Anchor }
    };
}
internal static class SeedDates
{
    public static readonly DateTime Anchor = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
}