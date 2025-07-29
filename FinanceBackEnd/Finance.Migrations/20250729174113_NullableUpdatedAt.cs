using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class NullableUpdatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var tables = new[]
            {
                "User", "Role", "ResourceOwner", "Resource", "MovementResource", "Movement",
                "IOLInvestmentResource", "IOLInvestmentAssetType", "IOLInvestmentAssetResource", "IOLInvestment",
                "IncomeResource", "Income", "IdentityProvider", "Identity", "FundResource", "Fund", "Frequency",
                "DebitResource", "DebitOriginResource", "CurrencyExchangeRateResource", "CurrencyExchangeRate",
                "CreditCardStatementResource", "CreditCardResource", "CreditCardMovementResource", "AppModuleType", "AppModule"
            };
            foreach (var table in tables)
            {
                MakeUpdatedAtNullable(migrationBuilder, table);
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var tables = new[]
            {
                "User", "Role", "ResourceOwner", "Resource", "MovementResource", "Movement",
                "IOLInvestmentResource", "IOLInvestmentAssetType", "IOLInvestmentAssetResource", "IOLInvestment",
                "IncomeResource", "Income", "IdentityProvider", "Identity", "FundResource", "Fund", "Frequency",
                "DebitResource", "DebitOriginResource", "CurrencyExchangeRateResource", "CurrencyExchangeRate",
                "CreditCardStatementResource", "CreditCardResource", "CreditCardMovementResource", "AppModuleType", "AppModule"
            };
            foreach (var table in tables)
            {
                MakeUpdatedAtNotNullable(migrationBuilder, table);
            }
        }

        private void MakeUpdatedAtNullable(MigrationBuilder migrationBuilder, string table)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: table,
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }

        private void MakeUpdatedAtNotNullable(MigrationBuilder migrationBuilder, string table)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: table,
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }
    }
}
