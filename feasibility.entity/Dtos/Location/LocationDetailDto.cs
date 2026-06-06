using feasibility.Entity.Dtos.Common;

namespace feasibility.Entity.Dtos.Location;

public class LocationDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string? Address { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public Guid? LocationTypeMaintenanceId { get; set; }
    public string? LocationTypeMaintenanceName { get; set; }
    public AuditInfoDto Audit { get; set; } = new();
}
