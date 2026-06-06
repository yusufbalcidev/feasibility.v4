using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feasibility.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class inittt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Pages",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreatedByName", "DeletedAt", "DeletedBy", "DeletedByName", "Description", "DisplayOrder", "Icon", "IsDeleted", "Key", "Name", "UpdatedAt", "UpdatedBy", "UpdatedByName" },
                values: new object[] { new Guid("11111111-1111-1111-1111-000000000007"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, null, null, 7, "ev_station", false, "Feasibility", "Fizibilite Projeleri", null, null, null });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "CanCreate", "CanDelete", "CanEdit", "CanView", "CreatedAt", "CreatedBy", "CreatedByName", "DeletedAt", "DeletedBy", "DeletedByName", "IsDeleted", "PageId", "RoleId", "UpdatedAt", "UpdatedBy", "UpdatedByName" },
                values: new object[] { new Guid("44444444-4444-4444-4444-000000000007"), true, true, true, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, null, false, new Guid("11111111-1111-1111-1111-000000000007"), new Guid("22222222-2222-2222-2222-000000000001"), null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-000000000007"));

            migrationBuilder.DeleteData(
                table: "Pages",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-000000000007"));
        }
    }
}
