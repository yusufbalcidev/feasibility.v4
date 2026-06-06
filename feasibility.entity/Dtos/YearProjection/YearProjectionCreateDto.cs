using System.ComponentModel.DataAnnotations;

namespace feasibility.Entity.Dtos.YearProjection;

public class YearProjectionCreateDto
{
    [Required(ErrorMessage = "Cihaz satırı seçimi zorunludur.")]
    public Guid DeviceLineId { get; set; }

    [Range(1, 50, ErrorMessage = "Yıl 1 ile 50 arasında olmalıdır.")]
    public int Year { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Günlük şarj tutarı negatif olamaz.")]
    public decimal DailyChargePerSocket { get; set; }
}
