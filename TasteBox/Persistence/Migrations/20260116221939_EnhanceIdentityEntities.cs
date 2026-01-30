using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasteBox.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EnhanceIdentityEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0197d227-ed75-7ac5-af55-31c0e1b6000f",
                columns: new[] { "Name", "NormalizedName" },
                values: new object[] { "Customer", "CUSTOMER" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0197d227-ed75-7ac5-af55-31ba464a746d",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEAxK00L83aA39Q3NkyhnUfr2TNVjk8wrFqHSZC228n+fKPIfl+xpSNLZV+21ptFudA==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0197d227-ed75-7ac5-af55-31c0e1b6000f",
                columns: new[] { "Name", "NormalizedName" },
                values: new object[] { "Member", "MEMBER" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0197d227-ed75-7ac5-af55-31ba464a746d",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEHwfVCg3j9s+4AGM15C0tJTpMhG76a2FPKTh1XuDvT0W9MZ+w6Lo2+zX8RR7ErPfSw==");
        }
    }
}
