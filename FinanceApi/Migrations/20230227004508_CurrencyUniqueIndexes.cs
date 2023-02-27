using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceApi.Migrations
{
    /// <inheritdoc />
    public partial class CurrencyUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Currency_Name",
                table: "Currency",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Currency_ShortName",
                table: "Currency",
                column: "ShortName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Currency_Name",
                table: "Currency");

            migrationBuilder.DropIndex(
                name: "IX_Currency_ShortName",
                table: "Currency");
        }
    }
}
