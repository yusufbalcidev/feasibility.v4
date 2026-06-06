using System.ComponentModel.DataAnnotations;

namespace feasibility.Entity.Dtos.LocationTypeMaintenance;

public class LocationTypeMaintenanceUpdateDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Tip adı zorunludur.")]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
}
