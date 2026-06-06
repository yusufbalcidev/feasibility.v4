namespace feasibility.Entity.Dtos.Role;

public class RolePagePermissionDto
{
    public Guid PageId { get; set; }
    public string PageKey { get; set; } = string.Empty;
    public string PageName { get; set; } = string.Empty;
    public bool CanView { get; set; }
    public bool CanCreate { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
}
