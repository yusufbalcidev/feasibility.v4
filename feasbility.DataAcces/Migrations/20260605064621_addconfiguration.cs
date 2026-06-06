using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feasibility.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addconfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeasibilityDeviceLines_FeasibilityStudies_StudyId",
                table: "FeasibilityDeviceLines");

            migrationBuilder.DropForeignKey(
                name: "FK_FeasibilityStudies_Locations_LocationId",
                table: "FeasibilityStudies");

            migrationBuilder.DropForeignKey(
                name: "FK_FeasibilityYearProjections_FeasibilityDeviceLines_DeviceLineId",
                table: "FeasibilityYearProjections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FeasibilityYearProjections",
                table: "FeasibilityYearProjections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FeasibilityStudies",
                table: "FeasibilityStudies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FeasibilityDeviceLines",
                table: "FeasibilityDeviceLines");

            migrationBuilder.RenameTable(
                name: "FeasibilityYearProjections",
                newName: "YearProjections");

            migrationBuilder.RenameTable(
                name: "FeasibilityStudies",
                newName: "Studies");

            migrationBuilder.RenameTable(
                name: "FeasibilityDeviceLines",
                newName: "DeviceLines");

            migrationBuilder.RenameIndex(
                name: "IX_FeasibilityYearProjections_DeviceLineId_Year",
                table: "YearProjections",
                newName: "IX_YearProjections_DeviceLineId_Year");

            migrationBuilder.RenameIndex(
                name: "IX_FeasibilityStudies_LocationId",
                table: "Studies",
                newName: "IX_Studies_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_FeasibilityStudies_CreatedAt",
                table: "Studies",
                newName: "IX_Studies_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_FeasibilityDeviceLines_StudyId_DeviceType",
                table: "DeviceLines",
                newName: "IX_DeviceLines_StudyId_DeviceType");

            migrationBuilder.AlterColumn<decimal>(
                name: "DailyChargePerSocket",
                table: "YearProjections",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "UsdRate",
                table: "Studies",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "StationUnitCostTl",
                table: "Studies",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "StationUnitCost",
                table: "Studies",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ProviderEntryFeeTl",
                table: "Studies",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ProviderEntryFee",
                table: "Studies",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PostWarrantyMaintenanceCostTl",
                table: "Studies",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PostWarrantyMaintenanceCost",
                table: "Studies",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MonthlyRentTl",
                table: "Studies",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MonthlyRent",
                table: "Studies",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "LoanAnnualInterestRate",
                table: "Studies",
                type: "decimal(8,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "LoanAmount",
                table: "Studies",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "InfrastructureCostTl",
                table: "Studies",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "InfrastructureCost",
                table: "Studies",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "InflationUsd",
                table: "Studies",
                type: "decimal(8,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "InflationTl",
                table: "Studies",
                type: "decimal(8,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "InflationEur",
                table: "Studies",
                type: "decimal(8,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,4)");

            migrationBuilder.AlterColumn<string>(
                name: "FeasibilityName",
                table: "Studies",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<decimal>(
                name: "EurRate",
                table: "Studies",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DeviceUnitCostTl",
                table: "Studies",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DeviceUnitCost",
                table: "Studies",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AdvertisingRevenueTl",
                table: "Studies",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AdvertisingRevenue",
                table: "Studies",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitLocationCostTl",
                table: "DeviceLines",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitLocationCost",
                table: "DeviceLines",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DailyChargesPerSocket",
                table: "DeviceLines",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AgreementRate",
                table: "DeviceLines",
                type: "decimal(8,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_YearProjections",
                table: "YearProjections",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Studies",
                table: "Studies",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeviceLines",
                table: "DeviceLines",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_YearProjections_DeviceLineId",
                table: "YearProjections",
                column: "DeviceLineId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceLines_StudyId",
                table: "DeviceLines",
                column: "StudyId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeviceLines_Studies_StudyId",
                table: "DeviceLines",
                column: "StudyId",
                principalTable: "Studies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Studies_Locations_LocationId",
                table: "Studies",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_YearProjections_DeviceLines_DeviceLineId",
                table: "YearProjections",
                column: "DeviceLineId",
                principalTable: "DeviceLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeviceLines_Studies_StudyId",
                table: "DeviceLines");

            migrationBuilder.DropForeignKey(
                name: "FK_Studies_Locations_LocationId",
                table: "Studies");

            migrationBuilder.DropForeignKey(
                name: "FK_YearProjections_DeviceLines_DeviceLineId",
                table: "YearProjections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_YearProjections",
                table: "YearProjections");

            migrationBuilder.DropIndex(
                name: "IX_YearProjections_DeviceLineId",
                table: "YearProjections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Studies",
                table: "Studies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeviceLines",
                table: "DeviceLines");

            migrationBuilder.DropIndex(
                name: "IX_DeviceLines_StudyId",
                table: "DeviceLines");

            migrationBuilder.RenameTable(
                name: "YearProjections",
                newName: "FeasibilityYearProjections");

            migrationBuilder.RenameTable(
                name: "Studies",
                newName: "FeasibilityStudies");

            migrationBuilder.RenameTable(
                name: "DeviceLines",
                newName: "FeasibilityDeviceLines");

            migrationBuilder.RenameIndex(
                name: "IX_YearProjections_DeviceLineId_Year",
                table: "FeasibilityYearProjections",
                newName: "IX_FeasibilityYearProjections_DeviceLineId_Year");

            migrationBuilder.RenameIndex(
                name: "IX_Studies_LocationId",
                table: "FeasibilityStudies",
                newName: "IX_FeasibilityStudies_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Studies_CreatedAt",
                table: "FeasibilityStudies",
                newName: "IX_FeasibilityStudies_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_DeviceLines_StudyId_DeviceType",
                table: "FeasibilityDeviceLines",
                newName: "IX_FeasibilityDeviceLines_StudyId_DeviceType");

            migrationBuilder.AlterColumn<decimal>(
                name: "DailyChargePerSocket",
                table: "FeasibilityYearProjections",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "UsdRate",
                table: "FeasibilityStudies",
                type: "decimal(10,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "StationUnitCostTl",
                table: "FeasibilityStudies",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "StationUnitCost",
                table: "FeasibilityStudies",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ProviderEntryFeeTl",
                table: "FeasibilityStudies",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ProviderEntryFee",
                table: "FeasibilityStudies",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PostWarrantyMaintenanceCostTl",
                table: "FeasibilityStudies",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PostWarrantyMaintenanceCost",
                table: "FeasibilityStudies",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MonthlyRentTl",
                table: "FeasibilityStudies",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MonthlyRent",
                table: "FeasibilityStudies",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "LoanAnnualInterestRate",
                table: "FeasibilityStudies",
                type: "decimal(5,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "LoanAmount",
                table: "FeasibilityStudies",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "InfrastructureCostTl",
                table: "FeasibilityStudies",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "InfrastructureCost",
                table: "FeasibilityStudies",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "InflationUsd",
                table: "FeasibilityStudies",
                type: "decimal(5,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "InflationTl",
                table: "FeasibilityStudies",
                type: "decimal(5,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "InflationEur",
                table: "FeasibilityStudies",
                type: "decimal(5,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,4)");

            migrationBuilder.AlterColumn<string>(
                name: "FeasibilityName",
                table: "FeasibilityStudies",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<decimal>(
                name: "EurRate",
                table: "FeasibilityStudies",
                type: "decimal(10,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DeviceUnitCostTl",
                table: "FeasibilityStudies",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DeviceUnitCost",
                table: "FeasibilityStudies",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AdvertisingRevenueTl",
                table: "FeasibilityStudies",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AdvertisingRevenue",
                table: "FeasibilityStudies",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitLocationCostTl",
                table: "FeasibilityDeviceLines",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitLocationCost",
                table: "FeasibilityDeviceLines",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DailyChargesPerSocket",
                table: "FeasibilityDeviceLines",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AgreementRate",
                table: "FeasibilityDeviceLines",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,4)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FeasibilityYearProjections",
                table: "FeasibilityYearProjections",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FeasibilityStudies",
                table: "FeasibilityStudies",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FeasibilityDeviceLines",
                table: "FeasibilityDeviceLines",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FeasibilityDeviceLines_FeasibilityStudies_StudyId",
                table: "FeasibilityDeviceLines",
                column: "StudyId",
                principalTable: "FeasibilityStudies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FeasibilityStudies_Locations_LocationId",
                table: "FeasibilityStudies",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FeasibilityYearProjections_FeasibilityDeviceLines_DeviceLineId",
                table: "FeasibilityYearProjections",
                column: "DeviceLineId",
                principalTable: "FeasibilityDeviceLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
