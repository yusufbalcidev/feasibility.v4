namespace feasibility.Entity.Dtos.FeasibilityAmortization;

public class FeasibilityAmortizationDetailDto
{
    public Guid Id { get; set; }
    public string FeasibilityName { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? CreatedByName { get; set; }
    public bool IsDeleted { get; set; }

    public decimal UsdRate { get; set; }
    public decimal EurRate { get; set; }
    public decimal InflationTl { get; set; }
    public decimal InflationUsd { get; set; }
    public decimal InflationEur { get; set; }

    public int ContractMonths { get; set; }
    public DateTime? ContractStartDate { get; set; }
    public decimal MonthlyLostDaysPercent { get; set; }

    public decimal StationUnitCostTl { get; set; }
    public decimal ProviderEntryFeeTl { get; set; }
    public decimal InfrastructureCostTl { get; set; }
    public decimal DeviceTotalTl { get; set; }
    public decimal TotalInvestmentTl { get; set; }
    public decimal TotalInvestmentUsd { get; set; }

    public decimal EquityInvestmentUsd { get; set; }

    public bool HasRent { get; set; }
    public decimal AnnualRentTl { get; set; }
    public decimal AnnualMaintenanceTl { get; set; }
    public decimal AnnualAdvertisingRevenueTl { get; set; }

    public bool HasLoan { get; set; }
    public decimal LoanAmount { get; set; }
    public decimal LoanAnnualInterestRate { get; set; }
    public int LoanTermMonths { get; set; }
    public decimal AnnualLoanPaymentTl { get; set; }

    public decimal LoanTotalInterestTl { get; set; }
    public decimal LoanTotalBsmTl { get; set; }
    public decimal LoanTotalRepaymentTl { get; set; }
    public decimal LoanMonthlyPaymentTl { get; set; }
    public List<LoanScheduleRowDto> LoanSchedule { get; set; } = new();

    public List<DeviceLineDetailDto> DeviceLines { get; set; } = new();
    public List<YearSummaryDto> YearSummaries { get; set; } = new();

    public decimal AnnualNetProfitTl { get; set; }
    public decimal AnnualNetProfitUsd { get; set; }
    public decimal PaybackYears { get; set; }
    public decimal RoiPercent { get; set; }

    public decimal RecoverableInvestmentUsd { get; set; }
    public decimal SunkInvestmentUsd { get; set; }
    public decimal SunkPaybackYears { get; set; }
    public List<SunkAmortizationRowDto> SunkAmortization { get; set; } = new();
}


public class LoanScheduleRowDto
{
    public int Month { get; set; }
    public decimal PrincipalTl { get; set; }
    public decimal InterestTl { get; set; }
    public decimal BsmTl { get; set; }
    public decimal InstallmentTl { get; set; }
    public decimal RemainingPrincipalTl { get; set; }
}

public class SunkAmortizationRowDto
{
    public int Year { get; set; }
    public decimal NetProfitUsd { get; set; }
    public decimal RecoveredCumulativeUsd { get; set; }
    public decimal RemainingUsd { get; set; }
    public bool IsPaybackYear { get; set; }
}

public class DeviceLineDetailDto
{
    public string DeviceType { get; set; } = string.Empty;
    public int DeviceCount { get; set; }
    public int SocketCount { get; set; }
    public decimal DailyChargesPerSocket { get; set; }
    public decimal AvgKwh { get; set; }
    public decimal SalePriceTl { get; set; }
    public decimal PurchasePriceTl { get; set; }
    public decimal UnitLocationCostTl { get; set; }
    public string AgreementGenre { get; set; } = string.Empty;
    public decimal AgreementRate { get; set; }
    public decimal LineInvestmentTl { get; set; }
    public decimal AnnualRevenueTl { get; set; }
    public decimal AnnualElectricityCostTl { get; set; }
    public decimal AnnualCommissionTl { get; set; }
    public decimal AnnualNetMarginTl { get; set; }
    public List<YearProjectionDetailDto> YearProjections { get; set; } = new();

    public List<MonthlyBreakdownYearDto> MonthlyBreakdownYears { get; set; } = new();
}

public class MonthlyBreakdownYearDto
{
    public int Year { get; set; }
    public decimal UsdRate { get; set; }
    public List<MonthlyBreakdownRowDto> Months { get; set; } = new();
}

public class MonthlyBreakdownRowDto
{
    public int Month { get; set; }
    public int DaysInMonth { get; set; }
    public decimal LostDays { get; set; }
    public decimal NetOperatingDays { get; set; }
    public decimal SaleKwhPerDevice { get; set; }

    public decimal SalePriceTlPerKwh { get; set; }
    public decimal PurchasePriceTlPerKwh { get; set; }

    public decimal RevenueTl { get; set; }
    public decimal RevenueUsd { get; set; }
    public decimal CommissionTl { get; set; }
    public decimal CommissionUsd { get; set; }
    public decimal RentTl { get; set; }
    public decimal RentUsd { get; set; }
    public decimal ElectricityCostTl { get; set; }
    public decimal ElectricityCostUsd { get; set; }
    public decimal GrossProfitTl { get; set; }
    public decimal GrossProfitUsd { get; set; }
}

public class YearProjectionDetailDto
{
    public int Year { get; set; }
    public decimal DailyChargePerSocket { get; set; }
    public decimal SalePriceH1 { get; set; }
    public decimal SalePriceH2 { get; set; }
    public decimal PurchasePriceH1 { get; set; }
    public decimal PurchasePriceH2 { get; set; }
    public decimal UsdRate { get; set; }
    public decimal AnnualRevenueTl { get; set; }
    public decimal AnnualElectricityCostTl { get; set; }
    public decimal AnnualCommissionTl { get; set; }
}

public class YearSummaryDto
{
    public int Year { get; set; }
    public decimal TotalRevenueTl { get; set; }
    public decimal TotalElectricityCostTl { get; set; }
    public decimal TotalCommissionTl { get; set; }
    public decimal FixedCostsTl { get; set; }
    public decimal NetProfitTl { get; set; }
    public decimal UsdRate { get; set; }
    public decimal NetProfitUsd { get; set; }
    public decimal CumulativeBalanceUsd { get; set; }
}
