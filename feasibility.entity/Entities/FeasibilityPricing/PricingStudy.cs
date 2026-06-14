using feasibility.Entity.Entities.Common;
using feasibility.Entity.Entities.Enums;

namespace feasibility.Entity.Entities.FeasibilityPricing;

public class PricingStudy : BaseEntity
{
    // ---- Temel bilgiler ----
    // Tarife fizibilitesi tüm Türkiye geneli geçerlidir; lokasyon bilgisi tutulmaz.
    public string FeasibilityName { get; set; } = string.Empty;

    public int Version { get; set; } = 1;

    public FeasibilityKind Kind { get; set; } = FeasibilityKind.Pricing;

    // ---- Kur ve enflasyon (amortisman fizibilitesi ile aynı) ----
    public decimal UsdRate { get; set; }

    public decimal EurRate { get; set; }

    public decimal InflationTl { get; set; }

    public decimal InflationUsd { get; set; }

    public decimal InflationEur { get; set; }

    // ---- Fiyatlandırma sabitleri ----
    // KDV oranı (%) — varsayılan 20
    public decimal VatRate { get; set; } = 20m;

    // Banka / eMSP komisyon oranı (%) — varsayılan 2,7
    public decimal CommissionRate { get; set; } = 2.7m;

    // ---- İstasyonlar ----
    public ICollection<PricingStation> Stations { get; set; } = new List<PricingStation>();
}
