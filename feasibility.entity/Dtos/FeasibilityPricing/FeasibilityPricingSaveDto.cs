using feasibility.Entity.Entities.Enums;

namespace feasibility.Entity.Dtos.FeasibilityPricing;

public class FeasibilityPricingSaveDto
{
    // ---- Temel bilgiler ----
    public string FeasibilityName { get; set; } = string.Empty;
    public FeasibilityKind Kind { get; set; } = FeasibilityKind.Pricing;

    public bool SaveAsNewVersion { get; set; }
    public int BaseVersion { get; set; }

    // ---- Kur ve enflasyon ----
    public decimal UsdRate { get; set; }
    public decimal EurRate { get; set; }

    public decimal InflationTl { get; set; }
    public decimal InflationUsd { get; set; }
    public decimal InflationEur { get; set; }

    // ---- Fiyatlandırma sabitleri ----
    public decimal VatRate { get; set; } = 20m;
    public decimal CommissionRate { get; set; } = 2.7m;

    // ---- İstasyonlar ----
    public List<PricingStationSaveDto> Stations { get; set; } = new();
}

public class PricingStationSaveDto
{
    // İstasyon bilgileri
    public DeviceType DeviceType { get; set; } = DeviceType.AC;
    public int SocketCount { get; set; }
    public decimal DailyKwhPerSocket { get; set; }
    public int EconomicLifeYears { get; set; }
    public decimal DiscountRate { get; set; }

    // Hedef kâr marjı / ROI (%)
    public decimal TargetProfitMargin { get; set; } = 10m;

    // CAPEX
    public decimal HardwareCost { get; set; }
    public CurrencyType HardwareCostCurrency { get; set; } = CurrencyType.TL;

    public decimal InfrastructureCost { get; set; }
    public CurrencyType InfrastructureCostCurrency { get; set; } = CurrencyType.TL;

    // OPEX
    public decimal AnnualOpex { get; set; }
    public CurrencyType AnnualOpexCurrency { get; set; } = CurrencyType.USD;

    // Şebeke elektrik birim maliyeti (EBM, USD/kWh) — senaryolar
    public decimal GridElectricityCost { get; set; }      // Ortalama (ana hesap)
    public decimal GridElectricityCostLow { get; set; }   // En düşük
    public decimal GridElectricityCostHigh { get; set; }  // En yüksek
}
