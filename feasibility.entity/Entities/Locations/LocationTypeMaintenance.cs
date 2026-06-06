using feasibility.Entity.Entities.Common;

namespace feasibility.Entity.Entities.Locations;

public class LocationTypeMaintenance : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<Location> Locations { get; set; } = new List<Location>();
}
