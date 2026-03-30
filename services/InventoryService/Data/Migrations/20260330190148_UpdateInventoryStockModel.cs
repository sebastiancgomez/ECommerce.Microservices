using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryService.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInventoryStockModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "InventoryItems");

            migrationBuilder.RenameColumn(
                name: "Stock",
                table: "InventoryItems",
                newName: "ReservedStock");

            migrationBuilder.AddColumn<int>(
                name: "AvailableStock",
                table: "InventoryItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableStock",
                table: "InventoryItems");

            migrationBuilder.RenameColumn(
                name: "ReservedStock",
                table: "InventoryItems",
                newName: "Stock");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "InventoryItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
