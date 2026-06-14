using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feasibility.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddPricingFeasibility : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PricingStudies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeasibilityName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Kind = table.Column<int>(type: "int", nullable: false),
                    UsdRate = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    EurRate = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    InflationTl = table.Column<decimal>(type: "decimal(8,4)", nullable: false),
                    InflationUsd = table.Column<decimal>(type: "decimal(8,4)", nullable: false),
                    InflationEur = table.Column<decimal>(type: "decimal(8,4)", nullable: false),
                    VatRate = table.Column<decimal>(type: "decimal(8,4)", nullable: false, defaultValue: 20m),
                    CommissionRate = table.Column<decimal>(type: "decimal(8,4)", nullable: false, defaultValue: 2.7m),
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
                    table.PrimaryKey("PK_PricingStudies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PricingStudies_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PricingStations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PricingStudyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceType = table.Column<int>(type: "int", nullable: false),
                    SocketCount = table.Column<int>(type: "int", nullable: false),
                    DailyKwhPerSocket = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    EconomicLifeYears = table.Column<int>(type: "int", nullable: false),
                    DiscountRate = table.Column<decimal>(type: "decimal(8,4)", nullable: false),
                    TargetProfitMargin = table.Column<decimal>(type: "decimal(8,4)", nullable: false, defaultValue: 10m),
                    HardwareCost = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    HardwareCostCurrency = table.Column<int>(type: "int", nullable: false),
                    HardwareCostTl = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    InfrastructureCost = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    InfrastructureCostCurrency = table.Column<int>(type: "int", nullable: false),
                    InfrastructureCostTl = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    AnnualOpex = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    AnnualOpexCurrency = table.Column<int>(type: "int", nullable: false),
                    AnnualOpexUsd = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GridElectricityCost = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    GridElectricityCostLow = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    GridElectricityCostHigh = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
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
                    table.PrimaryKey("PK_PricingStations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PricingStations_PricingStudies_PricingStudyId",
                        column: x => x.PricingStudyId,
                        principalTable: "PricingStudies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PricingStations_PricingStudyId",
                table: "PricingStations",
                column: "PricingStudyId");

            migrationBuilder.CreateIndex(
                name: "IX_PricingStudies_LocationId",
                table: "PricingStudies",
                column: "LocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PricingStations");

            migrationBuilder.DropTable(
                name: "PricingStudies");
        }
    }
}
