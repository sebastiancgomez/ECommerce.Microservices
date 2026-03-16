using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PricingService.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPricingRuleLifecycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "PricingRules",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "PricingRules",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "PricingRules",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "PricingRules");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "PricingRules");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "PricingRules");
        }
    }
}
