using System.Text;
using Finance.Persistence.Constants;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class CurrencySymbols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CurrencySymbols",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Symbol = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencySymbols", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurrencySymbols_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            var currencyQueryBuilder = new StringBuilder();
            var currencySymbolQueryBuilder = new StringBuilder();

            foreach (var currencyId in CurrencyConstants.CurrencyIds)
            {
                var currencyName = CurrencyConstants.Names[currencyId];
                var currencyShortName = CurrencyConstants.ShortNames[currencyId];

                currencyQueryBuilder.AppendLine($@"
                    IF NOT EXISTS (SELECT 1 FROM ""Currency"" WHERE ""Id"" = '{currencyId}') THEN
                        INSERT INTO ""Currency"" (""Id"", ""Deactivated"", ""Name"", ""ShortName"")
                        VALUES ('{currencyId}', false, '{currencyName}', '{currencyShortName}');
                    END IF;

                ");

                if (CurrencyConstants.CurrencySymbols.ContainsKey(currencyId))
                {
                    foreach (var symbol in CurrencyConstants.CurrencySymbols[currencyId])
                    {
                        var symbolId = symbol.Item1;
                        var symbolValue = symbol.Item2;

                        currencySymbolQueryBuilder.AppendLine($@"
                            IF NOT EXISTS (SELECT 1 FROM ""CurrencySymbols"" WHERE ""Id"" = '{symbolId}') THEN
                                INSERT INTO ""CurrencySymbols"" (""Id"", ""Deactivated"", ""Symbol"", ""CurrencyId"")
                                VALUES ('{symbolId}', false, '{symbolValue}', '{currencyId}');
                            END IF;
                        ");
                    }
                }
            }

            migrationBuilder.Sql($@"
            DO $$
            BEGIN
                {currencyQueryBuilder}
            END
            $$;
            ");

            migrationBuilder.Sql($@"
                DO $$
                BEGIN
                    {currencySymbolQueryBuilder}
                END
                $$;
            ");

            migrationBuilder.AddColumn<Guid>(
                name: "CurrencyId",
                table: "IOLInvestmentAsset",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid($"{CurrencyConstants.DefaultCurrencyId}"));

            migrationBuilder.CreateIndex(
                name: "IX_IOLInvestmentAsset_CurrencyId",
                table: "IOLInvestmentAsset",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Currency_Name_ShortName",
                table: "Currency",
                columns: ["Name", "ShortName"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CurrencySymbols_CurrencyId",
                table: "CurrencySymbols",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencySymbols_Symbol_CurrencyId",
                table: "CurrencySymbols",
                columns: ["Symbol", "CurrencyId"],
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_IOLInvestmentAsset_Currency_CurrencyId",
                table: "IOLInvestmentAsset",
                column: "CurrencyId",
                principalTable: "Currency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IOLInvestmentAsset_Currency_CurrencyId",
                table: "IOLInvestmentAsset");

            migrationBuilder.DropTable(
                name: "CurrencySymbols");

            migrationBuilder.DropIndex(
                name: "IX_IOLInvestmentAsset_CurrencyId",
                table: "IOLInvestmentAsset");

            migrationBuilder.DropIndex(
                name: "IX_Currency_Name_ShortName",
                table: "Currency");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "IOLInvestmentAsset");
        }
    }
}
