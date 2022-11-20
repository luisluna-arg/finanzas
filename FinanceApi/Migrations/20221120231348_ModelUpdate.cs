using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceApi.Migrations
{
    /// <inheritdoc />
    public partial class ModelUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CurrencyType",
                table: "Modules",
                newName: "CurrencyId");

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Modules_CurrencyId",
                table: "Modules",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Modules_Currencies_CurrencyId",
                table: "Modules",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modules_Currencies_CurrencyId",
                table: "Modules");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropIndex(
                name: "IX_Modules_CurrencyId",
                table: "Modules");

            migrationBuilder.RenameColumn(
                name: "CurrencyId",
                table: "Modules",
                newName: "CurrencyType");
        }
    }
}
