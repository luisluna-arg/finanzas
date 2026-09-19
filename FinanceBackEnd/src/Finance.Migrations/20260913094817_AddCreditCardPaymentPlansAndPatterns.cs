using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddCreditCardPaymentPlansAndPatterns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InstallmentNumber",
                table: "CreditCardTransaction",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentPlanId",
                table: "CreditCardTransaction",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InstallmentPatternId",
                table: "CreditCardStatementImportTemplate",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CreditCardInstallmentPattern",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RegexPattern = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsSystem = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCardInstallmentPattern", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditCardInstallmentPattern_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CreditCardPaymentPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BaseConcept = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    TotalInstallments = table.Column<int>(type: "integer", nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false),
                    CreditCardId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCardPaymentPlan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditCardPaymentPlan_CreditCard_CreditCardId",
                        column: x => x.CreditCardId,
                        principalTable: "CreditCard",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CreditCardPaymentPlan_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "CreditCardInstallmentPattern",
                columns: ["Id", "Name", "RegexPattern", "IsSystem", "UserId", "Deactivated", "CreatedAt", "UpdatedAt"],
                values: new object[,]
                {
                    { new Guid("26736ae5-04f5-48b6-b17d-c97dc6aa8cb0"), "Paréntesis (N/M)", @"^(?<base>.*?)\s*\(\s*(?<n>\d{1,2})\s*/\s*(?<m>\d{1,2})\s*\)\s*$", true, null, false, DateTime.UtcNow, null },
                    { new Guid("3705ef4c-947b-44e9-8079-8fc151a35386"), "Barra N/M", @"^(?<base>.*?)\s*(?<n>\d{1,2})\s*/\s*(?<m>\d{1,2})\s*$", true, null, false, DateTime.UtcNow, null },
                    { new Guid("1398abf0-2982-4820-bdb5-324544ccf609"), "Cuota N de M", @"^(?<base>.*?)\s*[Cc]uota\s*(?<n>\d{1,2})\s*de\s*(?<m>\d{1,2})\s*$", true, null, false, DateTime.UtcNow, null },
                });

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardTransaction_PaymentPlanId_InstallmentNumber",
                table: "CreditCardTransaction",
                columns: ["PaymentPlanId", "InstallmentNumber"]);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardStatementImportTemplate_InstallmentPatternId",
                table: "CreditCardStatementImportTemplate",
                column: "InstallmentPatternId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardInstallmentPattern_IsSystem_UserId",
                table: "CreditCardInstallmentPattern",
                columns: ["IsSystem", "UserId"]);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardInstallmentPattern_UserId",
                table: "CreditCardInstallmentPattern",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardPaymentPlan_CreditCardId_BaseConcept_TotalInstall~",
                table: "CreditCardPaymentPlan",
                columns: ["CreditCardId", "BaseConcept", "TotalInstallments", "CurrencyId"]);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardPaymentPlan_CurrencyId",
                table: "CreditCardPaymentPlan",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardStatementImportTemplate_CreditCardInstallmentPatt~",
                table: "CreditCardStatementImportTemplate",
                column: "InstallmentPatternId",
                principalTable: "CreditCardInstallmentPattern",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardTransaction_CreditCardPaymentPlan_PaymentPlanId",
                table: "CreditCardTransaction",
                column: "PaymentPlanId",
                principalTable: "CreditCardPaymentPlan",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardStatementImportTemplate_CreditCardInstallmentPatt~",
                table: "CreditCardStatementImportTemplate");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardTransaction_CreditCardPaymentPlan_PaymentPlanId",
                table: "CreditCardTransaction");

            migrationBuilder.DropTable(
                name: "CreditCardInstallmentPattern");

            migrationBuilder.DropTable(
                name: "CreditCardPaymentPlan");

            migrationBuilder.DropIndex(
                name: "IX_CreditCardTransaction_PaymentPlanId_InstallmentNumber",
                table: "CreditCardTransaction");

            migrationBuilder.DropIndex(
                name: "IX_CreditCardStatementImportTemplate_InstallmentPatternId",
                table: "CreditCardStatementImportTemplate");

            migrationBuilder.DropColumn(
                name: "InstallmentNumber",
                table: "CreditCardTransaction");

            migrationBuilder.DropColumn(
                name: "PaymentPlanId",
                table: "CreditCardTransaction");

            migrationBuilder.DropColumn(
                name: "InstallmentPatternId",
                table: "CreditCardStatementImportTemplate");
        }
    }
}
