using feasibility.Entity.Entities.Enums;

namespace feasibility.Entity.Dtos.Study;

public class StudyListDto
{
    public Guid Id { get; set; }
    public Guid LocationId { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public string FeasibilityName { get; set; } = string.Empty;
    public FeasibilityKind Kind { get; set; }
    public decimal UsdRate { get; set; }
    public decimal EurRate { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedByName { get; set; }
}
