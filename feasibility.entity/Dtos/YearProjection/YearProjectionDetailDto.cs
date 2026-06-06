using feasibility.Entity.Dtos.Common;

namespace feasibility.Entity.Dtos.YearProjection;

public class YearProjectionDetailDto
{
    public Guid Id { get; set; }
    public Guid DeviceLineId { get; set; }
    public int Year { get; set; }
    public decimal DailyChargePerSocket { get; set; }
    public AuditInfoDto Audit { get; set; } = new();
}
