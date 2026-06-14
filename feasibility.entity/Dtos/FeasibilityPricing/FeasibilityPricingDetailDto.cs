using feasibility.Entity.Entities.Enums;

namespace feasibility.Entity.Dtos.FeasibilityPricing;

public class FeasibilityPricingDetailDto
{
    public Guid Id { get; set; }
    public string FeasibilityName { get; set; } = string.Empty;
    public int Version { get; set; } = 1;
    public DateTime CreatedAt { get; set; }
    public string? CreatedByName { get; set; }
    public bool IsDeleted { get; set; }

    public decimal UsdRate { get; set; }
    public decimal EurRate { get; set; }
    public decimal InflationTl { get; set; }
    public decimal InflationUsd { get; set; }
    public decimal InflationEur { get; set; }

    // ---- Fiyatlandırma sabitleri ----
    public decimal VatRate { get; set; }
    public decimal CommissionRate { get; set; }

    public List<PricingStationDetailDto> Stations { get; set; } = new();
}

public class PricingStationDetailDto
{
    public string DeviceType { get; set; } = string.Empty;
    public int SocketCount { get; set; }
    public decimal DailyKwhPerSocket { get; set; }

    // Yıllık satış hacmi (kWh) = SocketCount * DailyKwhPerSocket * 365
    public decimal AnnualSalesKwh { get; set; }

    public int EconomicLifeYears { get; set; }
    public decimal DiscountRate { get; set; }

    // Hedef kâr marjı / ROI (%)
    public decimal TargetProfitMargin { get; set; }

    public decimal HardwareCost { get; set; }
    public CurrencyType HardwareCostCurrency { get; set; }
    public decimal HardwareCostTl { get; set; }

    public decimal InfrastructureCost { get; set; }
    public CurrencyType InfrastructureCostCurrency { get; set; }
    public decimal InfrastructureCostTl { get; set; }

    public decimal TotalCapexTl { get; set; }

    // ---- OPEX girdisi ----
    public decimal AnnualOpex { get; set; }
    public CurrencyType AnnualOpexCurrency { get; set; }
    public decimal AnnualOpexUsd { get; set; }

    // ---- EBM girdileri (USD/kWh) ----
    public decimal GridElectricityCost { get; set; }      // Ortalama
    public decimal GridElectricityCostLow { get; set; }   // En düşük
    public decimal GridElectricityCostHigh { get; set; }  // En yüksek

    // ---- Hesaplanan birim maliyet kalemleri (USD/kWh) ----
    public decimal AnnuityFactor { get; set; }            // r / (1 - (1+r)^-n)
    public decimal AnnualizedCapexUsd { get; set; }       // CAPEX_USD * annüite faktörü
    public decimal DepreciationShare { get; set; }        // Amortisman Payı
    public decimal OpexShare { get; set; }                // İşletme OPEX Payı
    public decimal EnergyShare { get; set; }              // Elektrik Enerji Payı (= ort. EBM)
    public decimal BaseCost { get; set; }                 // Taban maliyet (zarar etmeme sınırı)

    // ---- Satış fiyatları (USD/kWh) — ana hesap (ort. EBM) ----
    public decimal SalePriceExVat { get; set; }           // KDV hariç satış fiyatı
    public decimal RecommendedPrice { get; set; }         // Önerilen satış fiyatı (KDV dahil)

    // TL karşılıkları (önerilen fiyat)
    public decimal SalePriceExVatTl { get; set; }
    public decimal RecommendedPriceTl { get; set; }

    // ---- EBM senaryolu satış fiyatları (KDV dahil, USD/kWh) ----
    public decimal PriceLowEbm { get; set; }              // En düşük EBM'li fiyat
    public decimal PriceAvgEbm { get; set; }              // Ortalama EBM'li fiyat
    public decimal PriceHighEbm { get; set; }             // En yüksek EBM'li fiyat
}
