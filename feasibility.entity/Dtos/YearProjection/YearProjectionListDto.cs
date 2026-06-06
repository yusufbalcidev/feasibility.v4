namespace feasibility.Entity.Dtos.YearProjection;

public class YearProjectionListDto
{
    public Guid Id { get; set; }
    public Guid DeviceLineId { get; set; }
    public int Year { get; set; }
    public decimal DailyChargePerSocket { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedByName { get; set; }
}
