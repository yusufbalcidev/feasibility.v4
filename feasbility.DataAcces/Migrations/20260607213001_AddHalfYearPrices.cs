using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feasibility.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddHalfYearPrices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PurchasePriceH1",
                table: "YearProjections",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PurchasePriceH2",
                table: "YearProjections",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SalePriceH1",
                table: "YearProjections",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SalePriceH2",
                table: "YearProjections",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurchasePriceH1",
                table: "YearProjections");

            migrationBuilder.DropColumn(
                name: "PurchasePriceH2",
                table: "YearProjections");

            migrationBuilder.DropColumn(
                name: "SalePriceH1",
                table: "YearProjections");

            migrationBuilder.DropColumn(
                name: "SalePriceH2",
                table: "YearProjections");
        }
    }
}
