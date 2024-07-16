using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceApi.Domain.Migrations
{
    /// <inheritdoc />
    public partial class DeactivateField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Deactivated",
                table: "Movement",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deactivated",
                table: "IOLInvestments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deactivated",
                table: "IOLInvestmentAssetTypes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deactivated",
                table: "IOLInvestmentAssets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deactivated",
                table: "DebitOrigin",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deactivated",
                table: "Debit",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deactivated",
                table: "CurrencyConversion",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deactivated",
                table: "Currency",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deactivated",
                table: "Bank",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deactivated",
                table: "AppModule",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "IOLInvestmentAssetTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "Deactivated",
                value: false);

            migrationBuilder.UpdateData(
                table: "IOLInvestmentAssetTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "Deactivated",
                value: false);

            migrationBuilder.UpdateData(
                table: "IOLInvestmentAssetTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "Deactivated",
                value: false);

            migrationBuilder.UpdateData(
                table: "IOLInvestmentAssetTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "Deactivated",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deactivated",
                table: "Movement");

            migrationBuilder.DropColumn(
                name: "Deactivated",
                table: "IOLInvestments");

            migrationBuilder.DropColumn(
                name: "Deactivated",
                table: "IOLInvestmentAssetTypes");

            migrationBuilder.DropColumn(
                name: "Deactivated",
                table: "IOLInvestmentAssets");

            migrationBuilder.DropColumn(
                name: "Deactivated",
                table: "DebitOrigin");

            migrationBuilder.DropColumn(
                name: "Deactivated",
                table: "Debit");

            migrationBuilder.DropColumn(
                name: "Deactivated",
                table: "CurrencyConversion");

            migrationBuilder.DropColumn(
                name: "Deactivated",
                table: "Currency");

            migrationBuilder.DropColumn(
                name: "Deactivated",
                table: "Bank");

            migrationBuilder.DropColumn(
                name: "Deactivated",
                table: "AppModule");
        }
    }
}
