using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasteBox.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Products",
                newName: "PricePerBaseQuantity");

            migrationBuilder.RenameColumn(
                name: "ComparePrice",
                table: "Products",
                newName: "PriceAfterOffer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PricePerBaseQuantity",
                table: "Products",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "PriceAfterOffer",
                table: "Products",
                newName: "ComparePrice");
        }
    }
}
