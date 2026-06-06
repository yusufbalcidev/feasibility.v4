using feasibility.Entity.Dtos.Common;

namespace feasibility.Entity.Dtos.Role;

public class RoleDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int UserCount { get; set; }
    public bool IsSystem { get; set; }
    public List<RolePagePermissionDto> Permissions { get; set; } = new();
    public AuditInfoDto Audit { get; set; } = new();
}
