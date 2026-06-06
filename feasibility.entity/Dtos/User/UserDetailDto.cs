using feasibility.Entity.Dtos.Common;

namespace feasibility.Entity.Dtos.User;

public class UserDetailDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid? RoleId { get; set; }
    public string? RoleName { get; set; }
    public AuditInfoDto Audit { get; set; } = new();
}
