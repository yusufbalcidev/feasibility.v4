using feasibility.Entity.Dtos.Common;
using feasibility.Entity.Entities.Enums;

namespace feasibility.Entity.Dtos.DeviceLine;

public class DeviceLineDetailDto
{
    public Guid Id { get; set; }
    public Guid StudyId { get; set; }
    public string StudyName { get; set; } = string.Empty;
    public DeviceType DeviceType { get; set; }
    public int DeviceCount { get; set; }
    public int SocketCount { get; set; }
    public decimal DailyChargesPerSocket { get; set; }
    public decimal AvgKwh { get; set; }
    public decimal SalePriceTl { get; set; }
    public decimal PurchasePriceTl { get; set; }
    public decimal UnitLocationCost { get; set; }
    public CurrencyType UnitLocationCostCurrency { get; set; }
    public decimal UnitLocationCostTl { get; set; }
    public AgreementGenre AgreementGenre { get; set; }
    public decimal AgreementRate { get; set; }
    public AuditInfoDto Audit { get; set; } = new();
}
