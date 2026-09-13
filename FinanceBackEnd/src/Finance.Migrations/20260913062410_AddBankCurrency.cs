using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddBankCurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fund_Bank_BankId",
                table: "Fund");

            migrationBuilder.DropForeignKey(
                name: "FK_Fund_Currency_CurrencyId",
                table: "Fund");

            migrationBuilder.DropIndex(
                name: "IX_Fund_BankId",
                table: "Fund");

            migrationBuilder.CreateTable(
                name: "BankCurrency",
                columns: table => new
                {
                    BankId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    DailyUse = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankCurrency", x => new { x.BankId, x.CurrencyId });
                    table.ForeignKey(
                        name: "FK_BankCurrency_Bank_BankId",
                        column: x => x.BankId,
                        principalTable: "Bank",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BankCurrency_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BankCurrencyPermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BankId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionLevels = table.Column<int[]>(type: "integer[]", nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankCurrencyPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankCurrencyPermissions_BankCurrency_BankId_CurrencyId",
                        columns: x => new { x.BankId, x.CurrencyId },
                        principalTable: "BankCurrency",
                        principalColumns: ["BankId", "CurrencyId"],
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BankCurrencyPermissions_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankCurrency_CurrencyId",
                table: "BankCurrency",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_BankCurrencyPermissions_BankId_CurrencyId_UserId",
                table: "BankCurrencyPermissions",
                columns: ["BankId", "CurrencyId", "UserId"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankCurrencyPermissions_UserId",
                table: "BankCurrencyPermissions",
                column: "UserId");

            // Backfill one BankCurrency row per (BankId, CurrencyId) pair already used by Fund,
            // taking DailyUse from that pair's most recently timestamped Fund entry.
            migrationBuilder.Sql(@"
                INSERT INTO ""BankCurrency"" (""BankId"", ""CurrencyId"", ""DailyUse"", ""CreatedAt"", ""UpdatedAt"", ""Deactivated"")
                SELECT DISTINCT ON (f.""BankId"", f.""CurrencyId"")
                    f.""BankId"", f.""CurrencyId"", f.""DailyUse"", now(), NULL::timestamptz, false
                FROM ""Fund"" f
                ORDER BY f.""BankId"", f.""CurrencyId"", f.""TimeStamp"" DESC;
            ");

            // Backfill BankCurrencyPermissions: grant ownership of each pair to every user who
            // already owns a Fund entry for it.
            migrationBuilder.Sql(@"
                INSERT INTO ""BankCurrencyPermissions"" (""Id"", ""BankId"", ""CurrencyId"", ""UserId"", ""PermissionLevels"", ""Deactivated"", ""CreatedAt"", ""UpdatedAt"")
                SELECT DISTINCT gen_random_uuid(), f.""BankId"", f.""CurrencyId"", fp.""UserId"", ARRAY[3], false, now(), NULL::timestamptz
                FROM ""Fund"" f
                JOIN ""FundPermissions"" fp ON fp.""ResourceId"" = f.""Id""
                ON CONFLICT (""BankId"", ""CurrencyId"", ""UserId"") DO NOTHING;
            ");

            migrationBuilder.DropColumn(
                name: "DailyUse",
                table: "Fund");

            migrationBuilder.CreateIndex(
                name: "IX_Fund_BankId_CurrencyId",
                table: "Fund",
                columns: ["BankId", "CurrencyId"]);

            migrationBuilder.AddForeignKey(
                name: "FK_Fund_BankCurrency_BankId_CurrencyId",
                table: "Fund",
                columns: ["BankId", "CurrencyId"],
                principalTable: "BankCurrency",
                principalColumns: ["BankId", "CurrencyId"],
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Fund_Bank_BankId",
                table: "Fund",
                column: "BankId",
                principalTable: "Bank",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Fund_Currency_CurrencyId",
                table: "Fund",
                column: "CurrencyId",
                principalTable: "Currency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fund_BankCurrency_BankId_CurrencyId",
                table: "Fund");

            migrationBuilder.DropForeignKey(
                name: "FK_Fund_Bank_BankId",
                table: "Fund");

            migrationBuilder.DropForeignKey(
                name: "FK_Fund_Currency_CurrencyId",
                table: "Fund");

            migrationBuilder.DropTable(
                name: "BankCurrencyPermissions");

            migrationBuilder.DropTable(
                name: "BankCurrency");

            migrationBuilder.DropIndex(
                name: "IX_Fund_BankId_CurrencyId",
                table: "Fund");

            migrationBuilder.AddColumn<bool>(
                name: "DailyUse",
                table: "Fund",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Fund_BankId",
                table: "Fund",
                column: "BankId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fund_Bank_BankId",
                table: "Fund",
                column: "BankId",
                principalTable: "Bank",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Fund_Currency_CurrencyId",
                table: "Fund",
                column: "CurrencyId",
                principalTable: "Currency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
