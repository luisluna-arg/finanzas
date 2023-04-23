using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceApi.Migrations
{
    /// <inheritdoc />
    public partial class _2304230707RenameMovementColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyConversion_Currency_CurrencyId",
                table: "CurrencyConversion");

            migrationBuilder.RenameColumn(
                name: "Ammount",
                table: "Movement",
                newName: "Amount");

            migrationBuilder.AlterColumn<Guid>(
                name: "CurrencyId",
                table: "CurrencyConversion",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyConversion_Currency_CurrencyId",
                table: "CurrencyConversion",
                column: "CurrencyId",
                principalTable: "Currency",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyConversion_Currency_CurrencyId",
                table: "CurrencyConversion");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Movement",
                newName: "Ammount");

            migrationBuilder.AlterColumn<Guid>(
                name: "CurrencyId",
                table: "CurrencyConversion",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyConversion_Currency_CurrencyId",
                table: "CurrencyConversion",
                column: "CurrencyId",
                principalTable: "Currency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
