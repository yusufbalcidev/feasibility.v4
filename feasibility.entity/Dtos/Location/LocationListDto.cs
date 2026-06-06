namespace feasibility.Entity.Dtos.Location;

public class LocationListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string? LocationTypeMaintenanceName { get; set; }
    public Guid? LocationTypeMaintenanceId { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedByName { get; set; }
}
