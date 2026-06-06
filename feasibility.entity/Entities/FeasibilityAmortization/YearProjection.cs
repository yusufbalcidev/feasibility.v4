using feasibility.Entity.Entities.Common;

namespace feasibility.Entity.Entities.FeasibilityAmortization;
public class YearProjection : BaseEntity
{
    public Guid DeviceLineId { get; set; }
    public DeviceLine? DeviceLine { get; set; }
    public int Year { get; set; }
    public decimal DailyChargePerSocket { get; set; }
}
