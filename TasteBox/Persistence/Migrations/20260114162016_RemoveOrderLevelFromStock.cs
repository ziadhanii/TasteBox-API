using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasteBox.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOrderLevelFromStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReorderLevel",
                table: "Stock");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ReorderLevel",
                table: "Stock",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
