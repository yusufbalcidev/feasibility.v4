using feasibility.Entity.Entities.Authorization;

namespace feasibility.DataAccess.Seeds;

public static class RolePermissionSeed
{
    public static IReadOnlyList<RolePermission> All()
    {
        var pages = PageSeed.All();
        var list = new List<RolePermission>();
        int index = 0;
        foreach (var page in pages)
        {
            index++;
            list.Add(new RolePermission
            {
                Id = new Guid($"44444444-4444-4444-4444-{index:D12}"),
                RoleId = RoleSeed.SuperAdminRoleId,
                PageId = page.Id,
                CanView = true,
                CanCreate = true,
                CanEdit = true,
                CanDelete = true,
                CreatedAt = SeedDates.Anchor
            });
        }
        return list;
    }
}
