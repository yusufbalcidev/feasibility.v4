using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feasibility.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class PricingLocationManual : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PricingStudies_Locations_LocationId",
                table: "PricingStudies");

            migrationBuilder.DropIndex(
                name: "IX_PricingStudies_LocationId",
                table: "PricingStudies");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "PricingStudies");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "PricingStudies",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "PricingStudies",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LocationName",
                table: "PricingStudies",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "PricingStudies");

            migrationBuilder.DropColumn(
                name: "District",
                table: "PricingStudies");

            migrationBuilder.DropColumn(
                name: "LocationName",
                table: "PricingStudies");

            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "PricingStudies",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PricingStudies_LocationId",
                table: "PricingStudies",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_PricingStudies_Locations_LocationId",
                table: "PricingStudies",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
