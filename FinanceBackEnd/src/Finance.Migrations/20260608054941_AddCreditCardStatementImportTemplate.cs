using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddCreditCardStatementImportTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DefaultImportTemplateId",
                table: "CreditCard",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CreditCardStatementImportTemplate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsSystem = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ConfigJson = table.Column<string>(type: "text", nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCardStatementImportTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditCardStatementImportTemplate_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CreditCard_DefaultImportTemplateId",
                table: "CreditCard",
                column: "DefaultImportTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardStatementImportTemplate_IsSystem_UserId",
                table: "CreditCardStatementImportTemplate",
                columns: ["IsSystem", "UserId"]);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardStatementImportTemplate_UserId",
                table: "CreditCardStatementImportTemplate",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCard_CreditCardStatementImportTemplate_DefaultImportTemplateId",
                table: "CreditCard",
                column: "DefaultImportTemplateId",
                principalTable: "CreditCardStatementImportTemplate",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditCard_CreditCardStatementImportTemplate_DefaultImportTemplateId",
                table: "CreditCard");

            migrationBuilder.DropIndex(
                name: "IX_CreditCard_DefaultImportTemplateId",
                table: "CreditCard");

            migrationBuilder.DropTable(
                name: "CreditCardStatementImportTemplate");

            migrationBuilder.DropColumn(
                name: "DefaultImportTemplateId",
                table: "CreditCard");
        }
    }
}
