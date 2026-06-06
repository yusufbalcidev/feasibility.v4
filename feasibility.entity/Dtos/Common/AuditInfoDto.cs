namespace feasibility.Entity.Dtos.Common;

public class AuditInfoDto
{
    public DateTime CreatedAt { get; set; }
    public string? CreatedByName { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedByName { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedByName { get; set; }
}
