using feasibility.Entity.Entities.Common;

namespace feasibility.Entity.Entities.Locations;

public class Location : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string? Address { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }

    public Guid? LocationTypeMaintenanceId { get; set; }
    public LocationTypeMaintenance? LocationTypeMaintenance { get; set; }
}
