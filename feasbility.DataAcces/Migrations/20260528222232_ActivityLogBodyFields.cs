using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feasibility.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ActivityLogBodyFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequestBody",
                table: "ActivityLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestContentType",
                table: "ActivityLogs",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResponseBody",
                table: "ActivityLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResponseContentType",
                table: "ActivityLogs",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestBody",
                table: "ActivityLogs");

            migrationBuilder.DropColumn(
                name: "RequestContentType",
                table: "ActivityLogs");

            migrationBuilder.DropColumn(
                name: "ResponseBody",
                table: "ActivityLogs");

            migrationBuilder.DropColumn(
                name: "ResponseContentType",
                table: "ActivityLogs");
        }
    }
}
