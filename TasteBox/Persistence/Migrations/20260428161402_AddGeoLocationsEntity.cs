using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasteBox.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGeoLocationsEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GeoLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Governorate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeoLocations", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0197d227-ed75-7ac5-af55-31c0e1b6000f",
                column: "CreatedOn",
                value: new DateTime(2026, 4, 28, 16, 14, 0, 397, DateTimeKind.Utc).AddTicks(4053));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0197d227-ed75-7ac5-af55-31c2d55797c4",
                column: "CreatedOn",
                value: new DateTime(2026, 4, 28, 16, 14, 0, 397, DateTimeKind.Utc).AddTicks(3346));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0197d227-ed75-7ac5-af55-31ba464a746d",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEGk0oIGJW2kkkb2X3MPHMi6nvQq33puy2iZ93del0+etnJ8bfWgtYo8uZ2X+BCYurQ==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeoLocations");

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
    }
}
