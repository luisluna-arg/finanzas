using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class FixEntityResourceRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardMovementResource_CreditCardMovement_ResourceSourc~",
                table: "CreditCardMovementResource");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardResource_CreditCard_ResourceSourceId",
                table: "CreditCardResource");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardStatementResource_CreditCardStatement_ResourceSou~",
                table: "CreditCardStatementResource");

            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyExchangeRateResource_CurrencyExchangeRate_ResourceS~",
                table: "CurrencyExchangeRateResource");

            migrationBuilder.DropForeignKey(
                name: "FK_DebitResource_Debit_ResourceSourceId",
                table: "DebitResource");

            migrationBuilder.DropForeignKey(
                name: "FK_FundResource_Fund_ResourceSourceId",
                table: "FundResource");

            migrationBuilder.DropForeignKey(
                name: "FK_ResourceOwner_Resource_ResourceId",
                table: "ResourceOwner");

            migrationBuilder.DropForeignKey(
                name: "FK_ResourceOwner_User_UserId",
                table: "ResourceOwner");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardMovementResource_CreditCardMovement_ResourceSourc~",
                table: "CreditCardMovementResource",
                column: "ResourceSourceId",
                principalTable: "CreditCardMovement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardResource_CreditCard_ResourceSourceId",
                table: "CreditCardResource",
                column: "ResourceSourceId",
                principalTable: "CreditCard",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardStatementResource_CreditCardStatement_ResourceSou~",
                table: "CreditCardStatementResource",
                column: "ResourceSourceId",
                principalTable: "CreditCardStatement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyExchangeRateResource_CurrencyExchangeRate_ResourceS~",
                table: "CurrencyExchangeRateResource",
                column: "ResourceSourceId",
                principalTable: "CurrencyExchangeRate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DebitResource_Debit_ResourceSourceId",
                table: "DebitResource",
                column: "ResourceSourceId",
                principalTable: "Debit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FundResource_Fund_ResourceSourceId",
                table: "FundResource",
                column: "ResourceSourceId",
                principalTable: "Fund",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceOwner_Resource_ResourceId",
                table: "ResourceOwner",
                column: "ResourceId",
                principalTable: "Resource",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceOwner_User_UserId",
                table: "ResourceOwner",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardMovementResource_CreditCardMovement_ResourceSourc~",
                table: "CreditCardMovementResource");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardResource_CreditCard_ResourceSourceId",
                table: "CreditCardResource");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardStatementResource_CreditCardStatement_ResourceSou~",
                table: "CreditCardStatementResource");

            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyExchangeRateResource_CurrencyExchangeRate_ResourceS~",
                table: "CurrencyExchangeRateResource");

            migrationBuilder.DropForeignKey(
                name: "FK_DebitResource_Debit_ResourceSourceId",
                table: "DebitResource");

            migrationBuilder.DropForeignKey(
                name: "FK_FundResource_Fund_ResourceSourceId",
                table: "FundResource");

            migrationBuilder.DropForeignKey(
                name: "FK_ResourceOwner_Resource_ResourceId",
                table: "ResourceOwner");

            migrationBuilder.DropForeignKey(
                name: "FK_ResourceOwner_User_UserId",
                table: "ResourceOwner");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardMovementResource_CreditCardMovement_ResourceSourc~",
                table: "CreditCardMovementResource",
                column: "ResourceSourceId",
                principalTable: "CreditCardMovement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardResource_CreditCard_ResourceSourceId",
                table: "CreditCardResource",
                column: "ResourceSourceId",
                principalTable: "CreditCard",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardStatementResource_CreditCardStatement_ResourceSou~",
                table: "CreditCardStatementResource",
                column: "ResourceSourceId",
                principalTable: "CreditCardStatement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyExchangeRateResource_CurrencyExchangeRate_ResourceS~",
                table: "CurrencyExchangeRateResource",
                column: "ResourceSourceId",
                principalTable: "CurrencyExchangeRate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DebitResource_Debit_ResourceSourceId",
                table: "DebitResource",
                column: "ResourceSourceId",
                principalTable: "Debit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FundResource_Fund_ResourceSourceId",
                table: "FundResource",
                column: "ResourceSourceId",
                principalTable: "Fund",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceOwner_Resource_ResourceId",
                table: "ResourceOwner",
                column: "ResourceId",
                principalTable: "Resource",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceOwner_User_UserId",
                table: "ResourceOwner",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
