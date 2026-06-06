using feasibility.Entity.Entities.Common;
using feasibility.Entity.Entities.Identity;

namespace feasibility.Entity.Entities.Authorization;

public class RolePermission : BaseEntity
{
    public Guid RoleId { get; set; }
    public AppRole? Role { get; set; }

    public Guid PageId { get; set; }
    public Page? Page { get; set; }

    public bool CanView { get; set; }
    public bool CanCreate { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
}
