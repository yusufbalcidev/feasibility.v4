using feasibility.Entity.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace feasibility.Entity.Dtos.DeviceLine;

public class DeviceLineCreateDto
{
    [Required(ErrorMessage = "Fizibilite çalışması seçimi zorunludur.")]
    public Guid StudyId { get; set; }

    public DeviceType DeviceType { get; set; } = DeviceType.AC;

    [Range(1, int.MaxValue, ErrorMessage = "Cihaz adedi en az 1 olmalıdır.")]
    public int DeviceCount { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Soket adedi en az 1 olmalıdır.")]
    public int SocketCount { get; set; }

    public decimal DailyChargesPerSocket { get; set; }
    public decimal AvgKwh { get; set; }

    public decimal SalePriceTl { get; set; }
    public decimal PurchasePriceTl { get; set; }

    public decimal UnitLocationCost { get; set; }
    public CurrencyType UnitLocationCostCurrency { get; set; } = CurrencyType.TL;

    public AgreementGenre AgreementGenre { get; set; } = AgreementGenre.Profit;

    [Range(0, 1, ErrorMessage = "Anlaşma oranı 0 ile 1 arasında olmalıdır.")]
    public decimal AgreementRate { get; set; }
}
