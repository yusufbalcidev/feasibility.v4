using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feasibility.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class DropUniqueDeviceLineIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DeviceLines_StudyId_DeviceType",
                table: "DeviceLines");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DeviceLines_StudyId_DeviceType",
                table: "DeviceLines",
                columns: new[] { "StudyId", "DeviceType" },
                unique: true);
        }
    }
}
