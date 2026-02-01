using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TasteBox.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUnitTypeToUnit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Units",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Units",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Units",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0197d227-ed75-7ac5-af55-31ba464a746d",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEG6/tRQ37BfQoWNTH7ykonkbakX5QZqTDSxA1vWfT3ThBOAGTeVwJs1tcqbVtJpCYQ==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Units");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0197d227-ed75-7ac5-af55-31ba464a746d",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEM4SO5hX/JrqPHl7vuWu/MiQHlW4auKZcNMYtwJl2RIbXr5fxOQB3kXZibWhxvU7NQ==");
        }
    }
}
