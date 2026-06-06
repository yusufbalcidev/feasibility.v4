using feasibility.Entity.Dtos.Common;
using feasibility.Entity.Entities.Enums;

namespace feasibility.Entity.Dtos.Study;

public class StudyDetailDto
{
    public Guid Id { get; set; }
    public Guid LocationId { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public string FeasibilityName { get; set; } = string.Empty;
    public FeasibilityKind Kind { get; set; }

    public decimal UsdRate { get; set; }
    public decimal EurRate { get; set; }

    public decimal InflationTl { get; set; }
    public decimal InflationUsd { get; set; }
    public decimal InflationEur { get; set; }

    public bool HasRent { get; set; }
    public decimal MonthlyRent { get; set; }
    public CurrencyType RentCurrency { get; set; }
    public decimal MonthlyRentTl { get; set; }

    public decimal PostWarrantyMaintenanceCost { get; set; }
    public CurrencyType PostWarrantyMaintenanceCurrency { get; set; }
    public decimal PostWarrantyMaintenanceCostTl { get; set; }

    public decimal AdvertisingRevenue { get; set; }
    public CurrencyType AdvertisingRevenueCurrency { get; set; }
    public decimal AdvertisingRevenueTl { get; set; }

    public decimal StationUnitCost { get; set; }
    public CurrencyType StationUnitCostCurrency { get; set; }
    public decimal StationUnitCostTl { get; set; }

    public decimal ProviderEntryFee { get; set; }
    public CurrencyType ProviderEntryFeeCurrency { get; set; }
    public decimal ProviderEntryFeeTl { get; set; }

    public decimal InfrastructureCost { get; set; }
    public CurrencyType InfrastructureCostCurrency { get; set; }
    public decimal InfrastructureCostTl { get; set; }

    public decimal DeviceUnitCost { get; set; }
    public CurrencyType DeviceUnitCostCurrency { get; set; }
    public decimal DeviceUnitCostTl { get; set; }

    public bool HasLoan { get; set; }
    public decimal LoanAmount { get; set; }
    public decimal LoanAnnualInterestRate { get; set; }
    public int LoanTermMonths { get; set; }

    public AuditInfoDto Audit { get; set; } = new();
}
