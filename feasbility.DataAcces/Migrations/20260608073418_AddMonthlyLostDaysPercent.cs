using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feasibility.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddMonthlyLostDaysPercent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_YearProjections_DeviceLineId_Year",
                table: "YearProjections");

            migrationBuilder.DropIndex(
                name: "IX_Studies_CreatedAt",
                table: "Studies");

            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyLostDaysPercent",
                table: "Studies",
                type: "decimal(8,4)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MonthlyLostDaysPercent",
                table: "Studies");

            migrationBuilder.CreateIndex(
                name: "IX_YearProjections_DeviceLineId_Year",
                table: "YearProjections",
                columns: new[] { "DeviceLineId", "Year" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Studies_CreatedAt",
                table: "Studies",
                column: "CreatedAt");
        }
    }
}
