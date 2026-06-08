using feasibility.Entity.Entities.Enums;

namespace feasibility.Entity.Dtos.FeasibilityAmortization;

public class FeasibilityAmortizationSaveDto
{
    public Guid LocationId { get; set; }
    public string FeasibilityName { get; set; } = string.Empty;
    public FeasibilityKind Kind { get; set; } = FeasibilityKind.Amortization;

    public decimal UsdRate { get; set; }
    public decimal EurRate { get; set; }

    public decimal InflationTl { get; set; }
    public decimal InflationUsd { get; set; }
    public decimal InflationEur { get; set; }

    public bool HasRent { get; set; }
    public decimal MonthlyRent { get; set; }
    public CurrencyType RentCurrency { get; set; } = CurrencyType.TL;

    public int ContractMonths { get; set; }

    public decimal MonthlyLostDaysPercent { get; set; }

    public decimal PostWarrantyMaintenanceCost { get; set; }
    public CurrencyType PostWarrantyMaintenanceCurrency { get; set; } = CurrencyType.TL;

    public decimal AdvertisingRevenue { get; set; }
    public CurrencyType AdvertisingRevenueCurrency { get; set; } = CurrencyType.TL;

    public decimal StationUnitCost { get; set; }
    public CurrencyType StationUnitCostCurrency { get; set; } = CurrencyType.TL;

    public decimal ProviderEntryFee { get; set; }
    public CurrencyType ProviderEntryFeeCurrency { get; set; } = CurrencyType.TL;

    public decimal InfrastructureCost { get; set; }
    public CurrencyType InfrastructureCostCurrency { get; set; } = CurrencyType.TL;

    public decimal DeviceUnitCost { get; set; }
    public CurrencyType DeviceUnitCostCurrency { get; set; } = CurrencyType.TL;

    public bool HasLoan { get; set; }
    public decimal LoanAmount { get; set; }
    public decimal LoanAnnualInterestRate { get; set; }
    public int LoanTermMonths { get; set; }

    public List<DeviceLineSaveDto> DeviceLines { get; set; } = new();
}

public class DeviceLineSaveDto
{
    public DeviceType DeviceType { get; set; }
    public int DeviceCount { get; set; }
    public int SocketCount { get; set; }
    public decimal DailyChargesPerSocket { get; set; }
    public decimal AvgKwh { get; set; }
    public decimal SalePriceTl { get; set; }
    public decimal PurchasePriceTl { get; set; }
    public decimal UnitLocationCost { get; set; }
    public CurrencyType UnitLocationCostCurrency { get; set; } = CurrencyType.TL;
    public AgreementGenre AgreementGenre { get; set; } = AgreementGenre.Profit;
    public decimal AgreementRate { get; set; }
    public List<YearProjectionSaveDto> YearProjections { get; set; } = new();
}

public class YearProjectionSaveDto
{
    public int Year { get; set; }
    public decimal DailyChargePerSocket { get; set; }
    public decimal SalePriceH1 { get; set; }
    public decimal SalePriceH2 { get; set; }
    public decimal PurchasePriceH1 { get; set; }
    public decimal PurchasePriceH2 { get; set; }
    public decimal UsdRate { get; set; }
}
