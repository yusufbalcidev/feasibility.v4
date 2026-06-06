using feasibility.Entity.Entities.Common;

namespace feasibility.Entity.Entities.Authorization;

public class Page : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
