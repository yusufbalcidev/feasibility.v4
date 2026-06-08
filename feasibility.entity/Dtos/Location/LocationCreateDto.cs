using System.ComponentModel.DataAnnotations;

namespace feasibility.Entity.Dtos.Location;

public class LocationCreateDto
{
    [Required(ErrorMessage = "Lokasyon adı zorunludur.")]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "İl seçimi zorunludur.")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "İlçe seçimi zorunludur.")]
    public string District { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Address { get; set; }

    [Required(ErrorMessage = "Enlem zorunludur.")]
    public decimal Latitude { get; set; }

    [Required(ErrorMessage = "Boylam zorunludur.")]
    public decimal Longitude { get; set; }

    public Guid? LocationTypeMaintenanceId { get; set; }
}
