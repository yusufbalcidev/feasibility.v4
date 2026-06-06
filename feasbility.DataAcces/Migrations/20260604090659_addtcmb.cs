using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feasibility.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addtcmb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeasibilityYearProjections_FeasibilityStudies_StudyId",
                table: "FeasibilityYearProjections");

            migrationBuilder.DropColumn(
                name: "AvgChargeVolume",
                table: "FeasibilityStudies");

            migrationBuilder.DropColumn(
                name: "AvgDailyChargePerSocket",
                table: "FeasibilityStudies");

            migrationBuilder.DropColumn(
                name: "DailySalesPerDevice",
                table: "FeasibilityStudies");

            migrationBuilder.DropColumn(
                name: "DailySalesPerSocket",
                table: "FeasibilityStudies");

            migrationBuilder.DropColumn(
                name: "EurUsdParity",
                table: "FeasibilityStudies");

            migrationBuilder.DropColumn(
                name: "FinanceCostRate",
                table: "FeasibilityStudies");

            migrationBuilder.DropColumn(
                name: "LostDayRate",
                table: "FeasibilityStudies");

            migrationBuilder.DropColumn(
                name: "MaintenanceOpexRate",
                table: "FeasibilityStudies");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "FeasibilityStudies");

            migrationBuilder.RenameColumn(
                name: "StudyId",
                table: "FeasibilityYearProjections",
                newName: "DeviceLineId");

            migrationBuilder.RenameIndex(
                name: "IX_FeasibilityYearProjections_StudyId_Year",
                table: "FeasibilityYearProjections",
                newName: "IX_FeasibilityYearProjections_DeviceLineId_Year");

            migrationBuilder.RenameColumn(
                name: "StationCostUsd",
                table: "FeasibilityStudies",
                newName: "StationUnitCostTl");

            migrationBuilder.RenameColumn(
                name: "ProviderEntryCostUsd",
                table: "FeasibilityStudies",
                newName: "StationUnitCost");

            migrationBuilder.RenameColumn(
                name: "ProviderCommissionRate",
                table: "FeasibilityStudies",
                newName: "LoanAnnualInterestRate");

            migrationBuilder.RenameColumn(
                name: "PostWarrantyMaintenanceUsd",
                table: "FeasibilityStudies",
                newName: "ProviderEntryFeeTl");

            migrationBuilder.RenameColumn(
                name: "MonthlyRentUsd",
                table: "FeasibilityStudies",
                newName: "ProviderEntryFee");

            migrationBuilder.RenameColumn(
                name: "InfrastructureCostUsd",
                table: "FeasibilityStudies",
                newName: "PostWarrantyMaintenanceCostTl");

            migrationBuilder.RenameColumn(
                name: "ElectricityUnit",
                table: "FeasibilityStudies",
                newName: "StationUnitCostCurrency");

            migrationBuilder.RenameColumn(
                name: "ElectricityCurrency",
                table: "FeasibilityStudies",
                newName: "RentCurrency");

            migrationBuilder.RenameColumn(
                name: "ElectricityCost",
                table: "FeasibilityStudies",
                newName: "PostWarrantyMaintenanceCost");

            migrationBuilder.RenameColumn(
                name: "DcSalesUnit",
                table: "FeasibilityStudies",
                newName: "ProviderEntryFeeCurrency");

            migrationBuilder.RenameColumn(
                name: "DcSalesPrice",
                table: "FeasibilityStudies",
                newName: "MonthlyRentTl");

            migrationBuilder.RenameColumn(
                name: "DcSalesCurrency",
                table: "FeasibilityStudies",
                newName: "PostWarrantyMaintenanceCurrency");

            migrationBuilder.RenameColumn(
                name: "DailySalesPerSocketUnit",
                table: "FeasibilityStudies",
                newName: "LoanTermMonths");

            migrationBuilder.RenameColumn(
                name: "DailySalesPerDeviceUnit",
                table: "FeasibilityStudies",
                newName: "InfrastructureCostCurrency");

            migrationBuilder.RenameColumn(
                name: "CommissionBasis",
                table: "FeasibilityStudies",
                newName: "DeviceUnitCostCurrency");

            migrationBuilder.RenameColumn(
                name: "AvgChargeVolumeUnit",
                table: "FeasibilityStudies",
                newName: "AdvertisingRevenueCurrency");

            migrationBuilder.RenameColumn(
                name: "AdvertisingRevenueUsd",
                table: "FeasibilityStudies",
                newName: "MonthlyRent");

            migrationBuilder.AddColumn<decimal>(
                name: "AdvertisingRevenue",
                table: "FeasibilityStudies",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AdvertisingRevenueTl",
                table: "FeasibilityStudies",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DeviceUnitCost",
                table: "FeasibilityStudies",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DeviceUnitCostTl",
                table: "FeasibilityStudies",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "FeasibilityName",
                table: "FeasibilityStudies",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "HasLoan",
                table: "FeasibilityStudies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasRent",
                table: "FeasibilityStudies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "InfrastructureCost",
                table: "FeasibilityStudies",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "InfrastructureCostTl",
                table: "FeasibilityStudies",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LoanAmount",
                table: "FeasibilityStudies",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "AgreementGenre",
                table: "FeasibilityDeviceLines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "AgreementRate",
                table: "FeasibilityDeviceLines",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DailyChargesPerSocket",
                table: "FeasibilityDeviceLines",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PurchasePriceTl",
                table: "FeasibilityDeviceLines",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SalePriceTl",
                table: "FeasibilityDeviceLines",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitLocationCost",
                table: "FeasibilityDeviceLines",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "UnitLocationCostCurrency",
                table: "FeasibilityDeviceLines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitLocationCostTl",
                table: "FeasibilityDeviceLines",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddForeignKey(
                name: "FK_FeasibilityYearProjections_FeasibilityDeviceLines_DeviceLineId",
                table: "FeasibilityYearProjections",
                column: "DeviceLineId",
                principalTable: "FeasibilityDeviceLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeasibilityYearProjections_FeasibilityDeviceLines_DeviceLineId",
                table: "FeasibilityYearProjections");

            migrationBuilder.DropColumn(
                name: "AdvertisingRevenue",
                table: "FeasibilityStudies");

            migrationBuilder.DropColumn(
                name: "AdvertisingRevenueTl",
                table: "FeasibilityStudies");

            migrationBuilder.DropColumn(
                name: "DeviceUnitCost",
                table: "FeasibilityStudies");

            migrationBuilder.DropColumn(
                name: "DeviceUnitCostTl",
                table: "FeasibilityStudies");

            migrationBuilder.DropColumn(
                name: "FeasibilityName",
                table: "FeasibilityStudies");

            migrationBuilder.DropColumn(
                name: "HasLoan",
                table: "FeasibilityStudies");

            migrationBuilder.DropColumn(
                name: "HasRent",
                table: "FeasibilityStudies");

            migrationBuilder.DropColumn(
                name: "InfrastructureCost",
                table: "FeasibilityStudies");

            migrationBuilder.DropColumn(
                name: "InfrastructureCostTl",
                table: "FeasibilityStudies");

            migrationBuilder.DropColumn(
                name: "LoanAmount",
                table: "FeasibilityStudies");

            migrationBuilder.DropColumn(
                name: "AgreementGenre",
                table: "FeasibilityDeviceLines");

            migrationBuilder.DropColumn(
                name: "AgreementRate",
                table: "FeasibilityDeviceLines");

            migrationBuilder.DropColumn(
                name: "DailyChargesPerSocket",
                table: "FeasibilityDeviceLines");

            migrationBuilder.DropColumn(
                name: "PurchasePriceTl",
                table: "FeasibilityDeviceLines");

            migrationBuilder.DropColumn(
                name: "SalePriceTl",
                table: "FeasibilityDeviceLines");

            migrationBuilder.DropColumn(
                name: "UnitLocationCost",
                table: "FeasibilityDeviceLines");

            migrationBuilder.DropColumn(
                name: "UnitLocationCostCurrency",
                table: "FeasibilityDeviceLines");

            migrationBuilder.DropColumn(
                name: "UnitLocationCostTl",
                table: "FeasibilityDeviceLines");

            migrationBuilder.RenameColumn(
                name: "DeviceLineId",
                table: "FeasibilityYearProjections",
                newName: "StudyId");

            migrationBuilder.RenameIndex(
                name: "IX_FeasibilityYearProjections_DeviceLineId_Year",
                table: "FeasibilityYearProjections",
                newName: "IX_FeasibilityYearProjections_StudyId_Year");

            migrationBuilder.RenameColumn(
                name: "StationUnitCostTl",
                table: "FeasibilityStudies",
                newName: "StationCostUsd");

            migrationBuilder.RenameColumn(
                name: "StationUnitCostCurrency",
                table: "FeasibilityStudies",
                newName: "ElectricityUnit");

            migrationBuilder.RenameColumn(
                name: "StationUnitCost",
                table: "FeasibilityStudies",
                newName: "ProviderEntryCostUsd");

            migrationBuilder.RenameColumn(
                name: "RentCurrency",
                table: "FeasibilityStudies",
                newName: "ElectricityCurrency");

            migrationBuilder.RenameColumn(
                name: "ProviderEntryFeeTl",
                table: "FeasibilityStudies",
                newName: "PostWarrantyMaintenanceUsd");

            migrationBuilder.RenameColumn(
                name: "ProviderEntryFeeCurrency",
                table: "FeasibilityStudies",
                newName: "DcSalesUnit");

            migrationBuilder.RenameColumn(
                name: "ProviderEntryFee",
                table: "FeasibilityStudies",
                newName: "MonthlyRentUsd");

            migrationBuilder.RenameColumn(
                name: "PostWarrantyMaintenanceCurrency",
                table: "FeasibilityStudies",
                newName: "DcSalesCurrency");

            migrationBuilder.RenameColumn(
                name: "PostWarrantyMaintenanceCostTl",
                table: "FeasibilityStudies",
                newName: "InfrastructureCostUsd");

            migrationBuilder.RenameColumn(
                name: "PostWarrantyMaintenanceCost",
                table: "FeasibilityStudies",
                newName: "ElectricityCost");

            migrationBuilder.RenameColumn(
                name: "MonthlyRentTl",
                table: "FeasibilityStudies",
                newName: "DcSalesPrice");

            migrationBuilder.RenameColumn(
                name: "MonthlyRent",
                table: "FeasibilityStudies",
                newName: "AdvertisingRevenueUsd");

            migrationBuilder.RenameColumn(
                name: "LoanTermMonths",
                table: "FeasibilityStudies",
                newName: "DailySalesPerSocketUnit");

            migrationBuilder.RenameColumn(
                name: "LoanAnnualInterestRate",
                table: "FeasibilityStudies",
                newName: "ProviderCommissionRate");

            migrationBuilder.RenameColumn(
                name: "InfrastructureCostCurrency",
                table: "FeasibilityStudies",
                newName: "DailySalesPerDeviceUnit");

            migrationBuilder.RenameColumn(
                name: "DeviceUnitCostCurrency",
                table: "FeasibilityStudies",
                newName: "CommissionBasis");

            migrationBuilder.RenameColumn(
                name: "AdvertisingRevenueCurrency",
                table: "FeasibilityStudies",
                newName: "AvgChargeVolumeUnit");

            migrationBuilder.AddColumn<decimal>(
                name: "AvgChargeVolume",
                table: "FeasibilityStudies",
                type: "decimal(12,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AvgDailyChargePerSocket",
                table: "FeasibilityStudies",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DailySalesPerDevice",
                table: "FeasibilityStudies",
                type: "decimal(12,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DailySalesPerSocket",
                table: "FeasibilityStudies",
                type: "decimal(12,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "EurUsdParity",
                table: "FeasibilityStudies",
                type: "decimal(10,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinanceCostRate",
                table: "FeasibilityStudies",
                type: "decimal(5,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LostDayRate",
                table: "FeasibilityStudies",
                type: "decimal(5,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MaintenanceOpexRate",
                table: "FeasibilityStudies",
                type: "decimal(5,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "FeasibilityStudies",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_FeasibilityYearProjections_FeasibilityStudies_StudyId",
                table: "FeasibilityYearProjections",
                column: "StudyId",
                principalTable: "FeasibilityStudies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
