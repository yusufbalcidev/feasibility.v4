using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feasibility.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class EnableLockoutForAllUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-000000000001"),
                column: "LockoutEnabled",
                value: true);

            // Sonradan oluşturulan tüm kullanıcıları da güncelle
            migrationBuilder.Sql("UPDATE [Users] SET [LockoutEnabled] = 1 WHERE [LockoutEnabled] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-000000000001"),
                column: "LockoutEnabled",
                value: false);
        }
    }
}
