using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddCreditCardImportTemplateAssociation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CreditCardImportTemplate",
                columns: table => new
                {
                    CreditCardsId = table.Column<Guid>(type: "uuid", nullable: false),
                    ImportTemplatesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCardImportTemplate", x => new { x.CreditCardsId, x.ImportTemplatesId });
                    table.ForeignKey(
                        name: "FK_CreditCardImportTemplate_CreditCardStatementImportTemplate_~",
                        column: x => x.ImportTemplatesId,
                        principalTable: "CreditCardStatementImportTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreditCardImportTemplate_CreditCard_CreditCardsId",
                        column: x => x.CreditCardsId,
                        principalTable: "CreditCard",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardImportTemplate_ImportTemplatesId",
                table: "CreditCardImportTemplate",
                column: "ImportTemplatesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreditCardImportTemplate");
        }
    }
}
