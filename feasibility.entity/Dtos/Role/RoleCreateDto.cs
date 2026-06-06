using System.ComponentModel.DataAnnotations;

namespace feasibility.Entity.Dtos.Role;

public class RoleCreateDto
{
    [Required(ErrorMessage = "Rol adı zorunludur.")]
    [StringLength(80)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public List<RolePagePermissionDto> Permissions { get; set; } = new();
}
