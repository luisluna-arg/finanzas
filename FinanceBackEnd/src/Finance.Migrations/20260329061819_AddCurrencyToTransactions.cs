using Finance.Persistence.Constants;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencyToTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CurrencyId",
                table: "CreditCardTransaction",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid($"{CurrencyConstants.DefaultCurrencyId}"));

            migrationBuilder.AddColumn<Guid>(
                name: "CurrencyId",
                table: "CreditCardStatementTransaction",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid($"{CurrencyConstants.DefaultCurrencyId}"));

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardTransaction_CurrencyId",
                table: "CreditCardTransaction",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardStatementTransaction_CurrencyId",
                table: "CreditCardStatementTransaction",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardStatementTransaction_Currency_CurrencyId",
                table: "CreditCardStatementTransaction",
                column: "CurrencyId",
                principalTable: "Currency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardTransaction_Currency_CurrencyId",
                table: "CreditCardTransaction",
                column: "CurrencyId",
                principalTable: "Currency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardStatementTransaction_Currency_CurrencyId",
                table: "CreditCardStatementTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardTransaction_Currency_CurrencyId",
                table: "CreditCardTransaction");

            migrationBuilder.DropIndex(
                name: "IX_CreditCardTransaction_CurrencyId",
                table: "CreditCardTransaction");

            migrationBuilder.DropIndex(
                name: "IX_CreditCardStatementTransaction_CurrencyId",
                table: "CreditCardStatementTransaction");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "CreditCardTransaction");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "CreditCardStatementTransaction");
        }
    }
}
