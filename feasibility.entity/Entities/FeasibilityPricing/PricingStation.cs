using feasibility.Entity.Entities.Common;
using feasibility.Entity.Entities.Enums;

namespace feasibility.Entity.Entities.FeasibilityPricing;

public class PricingStation : BaseEntity
{
    public Guid PricingStudyId { get; set; }
    public PricingStudy? PricingStudy { get; set; }

    // ---- İstasyon bilgileri ----
    public DeviceType DeviceType { get; set; } = DeviceType.AC;

    public int SocketCount { get; set; }

    // Soket başı günlük kullanım (kWh). Yıllık satış hacmi = SocketCount * DailyKwhPerSocket * 365
    public decimal DailyKwhPerSocket { get; set; }

    // İstasyon ekonomik ömür (yıl)
    public int EconomicLifeYears { get; set; }

    // İskonto oranı (%)
    public decimal DiscountRate { get; set; }

    // Hedef kâr marjı / ROI (%) — CPO ve eMSP kâr beklentisi (çarpan)
    public decimal TargetProfitMargin { get; set; } = 10m;

    // ---- Yatırım maliyetleri (CAPEX) ----
    // Donanım maliyeti
    public decimal HardwareCost { get; set; }
    public CurrencyType HardwareCostCurrency { get; set; } = CurrencyType.TL;
    public decimal HardwareCostTl { get; set; }

    // Altyapı kurulum maliyeti
    public decimal InfrastructureCost { get; set; }
    public CurrencyType InfrastructureCostCurrency { get; set; } = CurrencyType.TL;
    public decimal InfrastructureCostTl { get; set; }

    // ---- İşletme giderleri (OPEX) ----
    // Yıllık sabit OPEX (CPO bakım + eMSP yazılım/çağrı merkezi)
    public decimal AnnualOpex { get; set; }
    public CurrencyType AnnualOpexCurrency { get; set; } = CurrencyType.USD;
    public decimal AnnualOpexUsd { get; set; }

    // ---- Şebeke elektrik birim maliyeti (EBM, USD/kWh) ----
    // Senaryo bazlı: En düşük / Ortalama (ana hesap) / En yüksek
    public decimal GridElectricityCost { get; set; }      // Ortalama EBM (ana hesap)
    public decimal GridElectricityCostLow { get; set; }   // En düşük EBM
    public decimal GridElectricityCostHigh { get; set; }  // En yüksek EBM
}
