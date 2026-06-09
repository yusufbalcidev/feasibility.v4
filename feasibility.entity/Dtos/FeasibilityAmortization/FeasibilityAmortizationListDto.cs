namespace feasibility.Entity.Dtos.FeasibilityAmortization;

public class FeasibilityAmortizationListDto
{
    public Guid Id { get; set; }
    public string FeasibilityName { get; set; } = string.Empty;
    public int Version { get; set; } = 1;

    /// <summary>Aynı ada sahip daha yüksek versiyon yoksa true. False ise kayıt salt-okunur (düzenlenemez).</summary>
    public bool IsLatest { get; set; } = true;
    public string LocationName { get; set; } = string.Empty;
    public List<string> DeviceTypes { get; set; } = new();
    public decimal TotalInvestmentUsd { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedByName { get; set; }
}
