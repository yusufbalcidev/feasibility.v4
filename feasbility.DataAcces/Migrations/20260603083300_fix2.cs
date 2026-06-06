using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feasibility.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class fix2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FeasibilityStudies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Kind = table.Column<int>(type: "int", nullable: false),
                    UsdRate = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    EurRate = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    EurUsdParity = table.Column<decimal>(type: "decimal(10,6)", nullable: false),
                    DcSalesPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DcSalesCurrency = table.Column<int>(type: "int", nullable: false),
                    DcSalesUnit = table.Column<int>(type: "int", nullable: false),
                    ElectricityCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ElectricityCurrency = table.Column<int>(type: "int", nullable: false),
                    ElectricityUnit = table.Column<int>(type: "int", nullable: false),
                    InflationTl = table.Column<decimal>(type: "decimal(5,4)", nullable: false),
                    InflationUsd = table.Column<decimal>(type: "decimal(5,4)", nullable: false),
                    InflationEur = table.Column<decimal>(type: "decimal(5,4)", nullable: false),
                    StationCostUsd = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProviderEntryCostUsd = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InfrastructureCostUsd = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MonthlyRentUsd = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProviderCommissionRate = table.Column<decimal>(type: "decimal(5,4)", nullable: false),
                    CommissionBasis = table.Column<int>(type: "int", nullable: false),
                    AvgDailyChargePerSocket = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    AvgChargeVolume = table.Column<decimal>(type: "decimal(12,4)", nullable: false),
                    AvgChargeVolumeUnit = table.Column<int>(type: "int", nullable: false),
                    DailySalesPerSocket = table.Column<decimal>(type: "decimal(12,4)", nullable: false),
                    DailySalesPerSocketUnit = table.Column<int>(type: "int", nullable: false),
                    DailySalesPerDevice = table.Column<decimal>(type: "decimal(12,4)", nullable: false),
                    DailySalesPerDeviceUnit = table.Column<int>(type: "int", nullable: false),
                    LostDayRate = table.Column<decimal>(type: "decimal(5,4)", nullable: false),
                    MaintenanceOpexRate = table.Column<decimal>(type: "decimal(5,4)", nullable: false),
                    FinanceCostRate = table.Column<decimal>(type: "decimal(5,4)", nullable: false),
                    PostWarrantyMaintenanceUsd = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AdvertisingRevenueUsd = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeasibilityStudies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeasibilityStudies_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FeasibilityDeviceLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceType = table.Column<int>(type: "int", nullable: false),
                    DeviceCount = table.Column<int>(type: "int", nullable: false),
                    SocketCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeasibilityDeviceLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeasibilityDeviceLines_FeasibilityStudies_StudyId",
                        column: x => x.StudyId,
                        principalTable: "FeasibilityStudies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeasibilityYearProjections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    DailyChargePerSocket = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeasibilityYearProjections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeasibilityYearProjections_FeasibilityStudies_StudyId",
                        column: x => x.StudyId,
                        principalTable: "FeasibilityStudies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeasibilityDeviceLines_StudyId_DeviceType",
                table: "FeasibilityDeviceLines",
                columns: new[] { "StudyId", "DeviceType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeasibilityStudies_CreatedAt",
                table: "FeasibilityStudies",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FeasibilityStudies_LocationId",
                table: "FeasibilityStudies",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_FeasibilityYearProjections_StudyId_Year",
                table: "FeasibilityYearProjections",
                columns: new[] { "StudyId", "Year" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeasibilityDeviceLines");

            migrationBuilder.DropTable(
                name: "FeasibilityYearProjections");

            migrationBuilder.DropTable(
                name: "FeasibilityStudies");
        }
    }
}
