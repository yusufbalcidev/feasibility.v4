namespace feasibility.Entity.Dtos.FeasibilityAmortization;

public class FeasibilityAmortizationListDto
{
    public Guid Id { get; set; }
    public string FeasibilityName { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public List<string> DeviceTypes { get; set; } = new();
    public decimal TotalInvestmentUsd { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedByName { get; set; }
}
