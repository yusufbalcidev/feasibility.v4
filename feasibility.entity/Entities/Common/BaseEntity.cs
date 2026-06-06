namespace feasibility.Entity.Entities.Common;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public string? CreatedByName { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string? UpdatedByName { get; set; }

    public bool IsDeleted { get; set; }=false;  
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    public string? DeletedByName { get; set; }
}
