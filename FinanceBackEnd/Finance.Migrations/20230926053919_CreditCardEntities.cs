using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class CreditCardEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CreditCardIssuers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BankId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCardIssuers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditCardIssuers_Bank_BankId",
                        column: x => x.BankId,
                        principalTable: "Bank",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreditCardMovements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditCardIssuerId = table.Column<Guid>(type: "uuid", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PlanStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Concept = table.Column<string>(type: "text", nullable: false),
                    PaymentNumber = table.Column<int>(type: "integer", nullable: false),
                    PlanSize = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    AmountDollars = table.Column<decimal>(type: "numeric", nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCardMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditCardMovements_CreditCardIssuers_CreditCardIssuerId",
                        column: x => x.CreditCardIssuerId,
                        principalTable: "CreditCardIssuers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardIssuers_BankId",
                table: "CreditCardIssuers",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardMovements_CreditCardIssuerId",
                table: "CreditCardMovements",
                column: "CreditCardIssuerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreditCardMovements");

            migrationBuilder.DropTable(
                name: "CreditCardIssuers");
        }
    }
}
