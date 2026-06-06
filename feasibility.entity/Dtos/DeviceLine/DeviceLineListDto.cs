using feasibility.Entity.Entities.Enums;

namespace feasibility.Entity.Dtos.DeviceLine;

public class DeviceLineListDto
{
    public Guid Id { get; set; }
    public Guid StudyId { get; set; }
    public string StudyName { get; set; } = string.Empty;
    public DeviceType DeviceType { get; set; }
    public int DeviceCount { get; set; }
    public int SocketCount { get; set; }
    public decimal DailyChargesPerSocket { get; set; }
    public decimal SalePriceTl { get; set; }
    public decimal PurchasePriceTl { get; set; }
    public AgreementGenre AgreementGenre { get; set; }
    public decimal AgreementRate { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedByName { get; set; }
}
