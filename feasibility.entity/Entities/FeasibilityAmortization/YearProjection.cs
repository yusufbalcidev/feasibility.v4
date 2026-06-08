using feasibility.Entity.Entities.Common;

namespace feasibility.Entity.Entities.FeasibilityAmortization;
public class YearProjection : BaseEntity
{
    public Guid DeviceLineId { get; set; }
    public DeviceLine? DeviceLine { get; set; }
    public int Year { get; set; }
    public decimal DailyChargePerSocket { get; set; }
    public decimal SalePriceH1 { get; set; }
    public decimal SalePriceH2 { get; set; }
    public decimal PurchasePriceH1 { get; set; }
    public decimal PurchasePriceH2 { get; set; }
    public decimal UsdRate { get; set; }
}
