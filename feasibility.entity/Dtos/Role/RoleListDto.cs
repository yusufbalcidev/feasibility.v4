namespace feasibility.Entity.Dtos.Role;

public class RoleListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int UserCount { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedByName { get; set; }
    public bool IsSystem { get; set; }
}
