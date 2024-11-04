using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class CreditCardStatement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreditCardStatementId",
                table: "CreditCard",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CreditCardStatement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditCardId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClosureDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiringDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCardStatement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditCardStatement_CreditCard_CreditCardId",
                        column: x => x.CreditCardId,
                        principalTable: "CreditCard",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardStatement_CreditCardId",
                table: "CreditCardStatement",
                column: "CreditCardId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreditCardStatement");

            migrationBuilder.DropColumn(
                name: "CreditCardStatementId",
                table: "CreditCard");
        }
    }
}
