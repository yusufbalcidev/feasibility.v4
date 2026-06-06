using feasibility.Entity.Entities.Identity;

namespace feasibility.DataAccess.Seeds;

public static class RoleSeed
{
    public static readonly Guid SuperAdminRoleId = new("22222222-2222-2222-2222-000000000001");

    public const string SuperAdminName = "SuperAdmin";

    public static IReadOnlyList<AppRole> All() => new List<AppRole>
    {
        new()
        {
            Id = SuperAdminRoleId,
            Name = SuperAdminName,
            NormalizedName = SuperAdminName.ToUpperInvariant(),
            Description = "Tüm yetkilere sahip sistem yöneticisi rolü.",
            ConcurrencyStamp = "ROLE-CONCURRENCY-SUPERADMIN",
            CreatedAt = SeedDates.Anchor
        }
    };
}
