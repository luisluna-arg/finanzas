using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class CurrencyExchangeRates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CurrencyExchangeRate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BaseCurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuoteCurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyRate = table.Column<decimal>(type: "numeric", nullable: false),
                    SellRate = table.Column<decimal>(type: "numeric", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyExchangeRate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurrencyExchangeRate_Currency_BaseCurrencyId",
                        column: x => x.BaseCurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CurrencyExchangeRate_Currency_QuoteCurrencyId",
                        column: x => x.QuoteCurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRate_BaseCurrencyId_QuoteCurrencyId_TimeSta~",
                table: "CurrencyExchangeRate",
                columns: ["BaseCurrencyId", "QuoteCurrencyId", "TimeStamp"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRate_QuoteCurrencyId",
                table: "CurrencyExchangeRate",
                column: "QuoteCurrencyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrencyExchangeRate");
        }
    }
}
