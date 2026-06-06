using feasibility.Entity.Dtos.Common;

namespace feasibility.Entity.Dtos.LocationTypeMaintenance;

public class LocationTypeMaintenanceDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int LocationCount { get; set; }
    public AuditInfoDto Audit { get; set; } = new();
}
