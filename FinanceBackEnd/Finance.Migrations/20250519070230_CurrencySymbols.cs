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
            migrationBuilder.AddColumn<Guid>(
                name: "CurrencyId",
                table: "IOLInvestmentAsset",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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

            migrationBuilder.InsertData(
                table: "Currency",
                columns: ["Id", "Deactivated", "Name", "ShortName"],
                values: new object[,]
                {
                    { new Guid("6d189135-7040-45a1-b713-b1aa6cad1720"), false, "Peso", "ARS" },
                    { new Guid("efbf50bc-34d4-43e9-96f9-9f6213ea11b5"), false, "Dollar", "USD" }
                });

            migrationBuilder.InsertData(
                table: "CurrencySymbols",
                columns: ["Id", "CurrencyId", "Deactivated", "Symbol"],
                values: new object[,]
                {
                    { new Guid("0a3d9502-aeec-4c35-92c1-9dc36c40612f"), new Guid("efbf50bc-34d4-43e9-96f9-9f6213ea11b5"), false, "USD" },
                    { new Guid("31d219c8-90a2-437b-8e52-f5fbf3bbd24f"), new Guid("6d189135-7040-45a1-b713-b1aa6cad1720"), false, "$" },
                    { new Guid("9b0ddd93-b13c-4443-ba97-0996672cbc1a"), new Guid("efbf50bc-34d4-43e9-96f9-9f6213ea11b5"), false, "U$D" },
                    { new Guid("9db01d76-76b0-438a-a6f3-38c4dda33ff4"), new Guid("efbf50bc-34d4-43e9-96f9-9f6213ea11b5"), false, "US$" }
                });

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

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: new Guid("6d189135-7040-45a1-b713-b1aa6cad1720"));

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: new Guid("efbf50bc-34d4-43e9-96f9-9f6213ea11b5"));

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "IOLInvestmentAsset");
        }
    }
}
