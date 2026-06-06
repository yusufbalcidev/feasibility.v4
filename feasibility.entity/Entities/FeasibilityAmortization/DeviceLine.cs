using feasibility.Entity.Entities.Common;
using feasibility.Entity.Entities.Enums;

namespace feasibility.Entity.Entities.FeasibilityAmortization;
public class DeviceLine : BaseEntity
{
    public Guid StudyId { get; set; }
    public Study? Study { get; set; }
    public DeviceType DeviceType { get; set; }
    public int DeviceCount { get; set; }
    public int SocketCount { get; set; }
    public decimal DailyChargesPerSocket { get; set; }
    public decimal AvgKwh { get; set; }
    public decimal SalePriceTl { get; set; }
    public decimal PurchasePriceTl { get; set; }
    public decimal UnitLocationCost { get; set; }
    public CurrencyType UnitLocationCostCurrency { get; set; } = CurrencyType.TL;
    public decimal UnitLocationCostTl { get; set; }
    public AgreementGenre AgreementGenre { get; set; } = AgreementGenre.Profit;
    public decimal AgreementRate { get; set; }
    public ICollection<YearProjection> YearProjections { get; set; } = new List<YearProjection>();
}
