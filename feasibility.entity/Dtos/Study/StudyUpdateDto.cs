using feasibility.Entity.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace feasibility.Entity.Dtos.Study;

public class StudyUpdateDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Lokasyon seçimi zorunludur.")]
    public Guid LocationId { get; set; }

    [Required(ErrorMessage = "Fizibilite adı zorunludur.")]
    [StringLength(200)]
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
}
