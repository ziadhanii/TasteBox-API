using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasteBox.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EnhanceOrderStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShippedAt",
                table: "Orders",
                newName: "PreparingAt");

            migrationBuilder.AddColumn<DateTime>(
                name: "OutForDeliveryAt",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0197d227-ed75-7ac5-af55-31c0e1b6000f",
                column: "CreatedOn",
                value: new DateTime(2026, 4, 28, 15, 18, 16, 521, DateTimeKind.Utc).AddTicks(4331));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0197d227-ed75-7ac5-af55-31c2d55797c4",
                column: "CreatedOn",
                value: new DateTime(2026, 4, 28, 15, 18, 16, 521, DateTimeKind.Utc).AddTicks(3909));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0197d227-ed75-7ac5-af55-31ba464a746d",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAED60aVU3o4t8BWLKQ0Ynd/Ol4E5SbbY0dizXUaxfp9hYgoyxxLrul+4AESI5PCFR9w==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OutForDeliveryAt",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "PreparingAt",
                table: "Orders",
                newName: "ShippedAt");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0197d227-ed75-7ac5-af55-31c0e1b6000f",
                column: "CreatedOn",
                value: new DateTime(2026, 4, 21, 17, 18, 8, 347, DateTimeKind.Utc).AddTicks(4632));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0197d227-ed75-7ac5-af55-31c2d55797c4",
                column: "CreatedOn",
                value: new DateTime(2026, 4, 21, 17, 18, 8, 347, DateTimeKind.Utc).AddTicks(3919));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0197d227-ed75-7ac5-af55-31ba464a746d",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEAFr5OboVM2A85W+JFRRKSJEQrs0wKgnALYPhY5NPNeVcdu3HFoHVgRnfpyEdscQjg==");
        }
    }
}
