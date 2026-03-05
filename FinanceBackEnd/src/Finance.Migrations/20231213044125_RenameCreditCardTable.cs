using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class RenameCreditCardTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "CreditCardIssuers",
                newName: "CreditCard");

            migrationBuilder.RenameColumn(
                table: "CreditCardMovements",
                name: "CreditCardIssuerId",
                newName: "CreditCardId");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardIssuers_Bank_BankId",
                table: "CreditCard");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardMovements_CreditCardIssuers_CreditCardIssuerId",
                table: "CreditCardMovements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CreditCardIssuers",
                table: "CreditCard");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CreditCard",
                column: "Id",
                table: "CreditCard");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCard_Bank_BankId",
                table: "CreditCard",
                principalTable: "Bank",
                column: "BankId",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardMovements_CreditCard_CreditCardId",
                table: "CreditCardMovements",
                principalTable: "CreditCard",
                column: "CreditCardId",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.RenameIndex(
                table: "CreditCardMovements",
                name: "IX_CreditCardMovements_CreditCardIssuerId",
                newName: "IX_CreditCardMovements_CreditCardId");

            migrationBuilder.RenameIndex(
                table: "CreditCard",
                name: "IX_CreditCardIssuers_BankId",
                newName: "IX_CreditCard_BankId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "CreditCard",
                newName: "CreditCardIssuers");

            migrationBuilder.RenameColumn(
                table: "CreditCardMovements",
                name: "CreditCardId",
                newName: "CreditCardIssuerId");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCard_Bank_BankId",
                table: "CreditCardIssuers");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardMovements_CreditCard_CreditCardId",
                table: "CreditCardMovements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CreditCard",
                table: "CreditCardIssuers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CreditCardIssuers",
                column: "Id",
                table: "CreditCardIssuers");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardIssuers_Bank_BankId",
                table: "CreditCardIssuers",
                principalTable: "Bank",
                column: "BankId",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardMovements_CreditCardIssuers_CreditCardIssuerId",
                table: "CreditCardMovements",
                principalTable: "CreditCardIssuers",
                column: "CreditCardIssuerId",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.RenameIndex(
                table: "CreditCardMovements",
                name: "IX_CreditCardMovements_CreditCardId",
                newName: "IX_CreditCardMovements_CreditCardIssuerId");

            migrationBuilder.RenameIndex(
                table: "CreditCard",
                name: "IX_CreditCard_BankId",
                newName: "IX_CreditCardIssuers_BankId");
        }
    }
}
