using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasteBox.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditToCartAndOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0197d227-ed75-7ac5-af55-31c0e1b6000f",
                column: "CreatedOn",
                value: new DateTime(2026, 2, 2, 22, 37, 18, 426, DateTimeKind.Utc).AddTicks(5712));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0197d227-ed75-7ac5-af55-31c2d55797c4",
                column: "CreatedOn",
                value: new DateTime(2026, 2, 2, 22, 37, 18, 426, DateTimeKind.Utc).AddTicks(4929));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0197d227-ed75-7ac5-af55-31ba464a746d",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEMcR7DeFGmsWa/cjKsIG4ZIj5YQeCTn58+8PL1Z/29moLu0arW3epLujLEbl02tEUA==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0197d227-ed75-7ac5-af55-31c0e1b6000f",
                column: "CreatedOn",
                value: new DateTime(2026, 2, 2, 22, 24, 4, 872, DateTimeKind.Utc).AddTicks(1833));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0197d227-ed75-7ac5-af55-31c2d55797c4",
                column: "CreatedOn",
                value: new DateTime(2026, 2, 2, 22, 24, 4, 872, DateTimeKind.Utc).AddTicks(1062));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0197d227-ed75-7ac5-af55-31ba464a746d",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEJWIdq/MNoBavbJ1rWJOoYwiGzy/2gtWec6k91uI4CYoJ0nES11n33fegMiSz1iTIQ==");
        }
    }
}
