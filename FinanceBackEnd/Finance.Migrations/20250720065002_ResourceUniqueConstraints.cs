using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class ResourceUniqueConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ResourceOwner_UserId",
                table: "ResourceOwner");

            migrationBuilder.DropIndex(
                name: "IX_FundResource_ResourceId",
                table: "FundResource");

            migrationBuilder.DropIndex(
                name: "IX_DebitResource_ResourceId",
                table: "DebitResource");

            migrationBuilder.DropIndex(
                name: "IX_CurrencyExchangeRateResource_ResourceId",
                table: "CurrencyExchangeRateResource");

            migrationBuilder.DropIndex(
                name: "IX_CreditCardStatementResource_ResourceId",
                table: "CreditCardStatementResource");

            migrationBuilder.DropIndex(
                name: "IX_CreditCardResource_ResourceId",
                table: "CreditCardResource");

            migrationBuilder.DropIndex(
                name: "IX_CreditCardMovementResource_ResourceId",
                table: "CreditCardMovementResource");

            migrationBuilder.CreateIndex(
                name: "IX_User_Username",
                table: "User",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResourceOwner_UserId_ResourceId",
                table: "ResourceOwner",
                columns: ["UserId", "ResourceId"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Identity_SourceId",
                table: "Identity",
                column: "SourceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FundResource_ResourceId_ResourceSourceId",
                table: "FundResource",
                columns: ["ResourceId", "ResourceSourceId"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DebitResource_ResourceId_ResourceSourceId",
                table: "DebitResource",
                columns: ["ResourceId", "ResourceSourceId"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRateResource_ResourceId_ResourceSourceId",
                table: "CurrencyExchangeRateResource",
                columns: ["ResourceId", "ResourceSourceId"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardStatementResource_ResourceId_ResourceSourceId",
                table: "CreditCardStatementResource",
                columns: ["ResourceId", "ResourceSourceId"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardResource_ResourceId_ResourceSourceId",
                table: "CreditCardResource",
                columns: ["ResourceId", "ResourceSourceId"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardMovementResource_ResourceId_ResourceSourceId",
                table: "CreditCardMovementResource",
                columns: ["ResourceId", "ResourceSourceId"],
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_Username",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_ResourceOwner_UserId_ResourceId",
                table: "ResourceOwner");

            migrationBuilder.DropIndex(
                name: "IX_Identity_SourceId",
                table: "Identity");

            migrationBuilder.DropIndex(
                name: "IX_FundResource_ResourceId_ResourceSourceId",
                table: "FundResource");

            migrationBuilder.DropIndex(
                name: "IX_DebitResource_ResourceId_ResourceSourceId",
                table: "DebitResource");

            migrationBuilder.DropIndex(
                name: "IX_CurrencyExchangeRateResource_ResourceId_ResourceSourceId",
                table: "CurrencyExchangeRateResource");

            migrationBuilder.DropIndex(
                name: "IX_CreditCardStatementResource_ResourceId_ResourceSourceId",
                table: "CreditCardStatementResource");

            migrationBuilder.DropIndex(
                name: "IX_CreditCardResource_ResourceId_ResourceSourceId",
                table: "CreditCardResource");

            migrationBuilder.DropIndex(
                name: "IX_CreditCardMovementResource_ResourceId_ResourceSourceId",
                table: "CreditCardMovementResource");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceOwner_UserId",
                table: "ResourceOwner",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FundResource_ResourceId",
                table: "FundResource",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_DebitResource_ResourceId",
                table: "DebitResource",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRateResource_ResourceId",
                table: "CurrencyExchangeRateResource",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardStatementResource_ResourceId",
                table: "CreditCardStatementResource",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardResource_ResourceId",
                table: "CreditCardResource",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardMovementResource_ResourceId",
                table: "CreditCardMovementResource",
                column: "ResourceId");
        }
    }
}
