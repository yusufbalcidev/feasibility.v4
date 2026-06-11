using feasibility.Entity.Entities.Common;
using feasibility.Entity.Entities.Enums;
using feasibility.Entity.Entities.Locations;

namespace feasibility.Entity.Entities.FeasibilityAmortization;

public class Study : BaseEntity
{

    public Guid LocationId { get; set; }
    public Location? Location { get; set; }

    public string FeasibilityName { get; set; } = string.Empty;

    public int Version { get; set; } = 1;

    public FeasibilityKind Kind { get; set; } = FeasibilityKind.Amortization;


    public decimal UsdRate { get; set; }

    public decimal EurRate { get; set; }


    public decimal InflationTl { get; set; }

    public decimal InflationUsd { get; set; }

    public decimal InflationEur { get; set; }


    public bool HasRent { get; set; }

    public decimal MonthlyRent { get; set; }
    public CurrencyType RentCurrency { get; set; } = CurrencyType.TL;

    public decimal MonthlyRentTl { get; set; }


    public int ContractMonths { get; set; }

    public DateTime? ContractStartDate { get; set; }

    public decimal MonthlyLostDaysPercent { get; set; }

    public decimal PostWarrantyMaintenanceCost { get; set; }
    public CurrencyType PostWarrantyMaintenanceCurrency { get; set; } = CurrencyType.TL;
    public decimal PostWarrantyMaintenanceCostTl { get; set; }

    public decimal AdvertisingRevenue { get; set; }
    public CurrencyType AdvertisingRevenueCurrency { get; set; } = CurrencyType.TL;
    public decimal AdvertisingRevenueTl { get; set; }

    public decimal StationUnitCost { get; set; }
    public CurrencyType StationUnitCostCurrency { get; set; } = CurrencyType.TL;
    public decimal StationUnitCostTl { get; set; }

    public decimal ProviderEntryFee { get; set; }
    public CurrencyType ProviderEntryFeeCurrency { get; set; } = CurrencyType.TL;
    public decimal ProviderEntryFeeTl { get; set; }

    public decimal InfrastructureCost { get; set; }
    public CurrencyType InfrastructureCostCurrency { get; set; } = CurrencyType.TL;
    public decimal InfrastructureCostTl { get; set; }

    public decimal DeviceUnitCost { get; set; }
    public CurrencyType DeviceUnitCostCurrency { get; set; } = CurrencyType.TL;
    public decimal DeviceUnitCostTl { get; set; }


    public bool HasLoan { get; set; }

    public decimal LoanAmount { get; set; }

    public decimal LoanAnnualInterestRate { get; set; }

    public int LoanTermMonths { get; set; }

    public ICollection<DeviceLine> DeviceLines { get; set; } = new List<DeviceLine>();
}
