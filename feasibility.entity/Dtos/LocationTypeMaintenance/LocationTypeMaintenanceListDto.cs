namespace feasibility.Entity.Dtos.LocationTypeMaintenance;

public class LocationTypeMaintenanceListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedByName { get; set; }
    public int LocationCount { get; set; }
}
