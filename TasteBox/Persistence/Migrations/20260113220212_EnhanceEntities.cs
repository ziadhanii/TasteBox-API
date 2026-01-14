using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasteBox.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EnhanceEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseQuantity",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "LastRestocked",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "LowStockThreshold",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "ReorderPoint",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "ReorderQuantity",
                table: "Inventories");

            migrationBuilder.RenameColumn(
                name: "PricePerBaseQuantity",
                table: "Products",
                newName: "UnitPrice");

            migrationBuilder.RenameColumn(
                name: "PriceAfterOffer",
                table: "Products",
                newName: "DiscountedPrice");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Inventories",
                newName: "LastUpdated");

            migrationBuilder.RenameColumn(
                name: "ReservedQuantity",
                table: "Inventories",
                newName: "MinQuantity");

            migrationBuilder.AlterColumn<decimal>(
                name: "CostPrice",
                table: "Products",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "Products",
                newName: "PricePerBaseQuantity");

            migrationBuilder.RenameColumn(
                name: "DiscountedPrice",
                table: "Products",
                newName: "PriceAfterOffer");

            migrationBuilder.RenameColumn(
                name: "MinQuantity",
                table: "Inventories",
                newName: "ReservedQuantity");

            migrationBuilder.RenameColumn(
                name: "LastUpdated",
                table: "Inventories",
                newName: "UpdatedAt");

            migrationBuilder.AlterColumn<decimal>(
                name: "CostPrice",
                table: "Products",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AddColumn<decimal>(
                name: "BaseQuantity",
                table: "Products",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Inventories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Inventories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastRestocked",
                table: "Inventories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LowStockThreshold",
                table: "Inventories",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ReorderPoint",
                table: "Inventories",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ReorderQuantity",
                table: "Inventories",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
