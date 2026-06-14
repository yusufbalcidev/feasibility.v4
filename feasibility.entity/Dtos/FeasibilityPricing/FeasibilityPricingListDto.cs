namespace feasibility.Entity.Dtos.FeasibilityPricing;

public class FeasibilityPricingListDto
{
    public Guid Id { get; set; }
    public string FeasibilityName { get; set; } = string.Empty;
    public int Version { get; set; } = 1;

    public bool IsLatest { get; set; } = true;
    public List<string> DeviceTypes { get; set; } = new();
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedByName { get; set; }
}
