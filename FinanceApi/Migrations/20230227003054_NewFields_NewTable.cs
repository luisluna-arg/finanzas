using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceApi.Migrations
{
    /// <inheritdoc />
    public partial class NewFieldsNewTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Fund",
                table: "Fund");

            migrationBuilder.RenameTable(
                name: "Fund",
                newName: "ModuleEntry");

            migrationBuilder.AddColumn<Guid>(
                name: "CurrencyId",
                table: "Movement",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortName",
                table: "Currency",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ModuleEntry",
                table: "ModuleEntry",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CurrencyConversion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MovementId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyConversion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurrencyConversion_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CurrencyConversion_Movement_MovementId",
                        column: x => x.MovementId,
                        principalTable: "Movement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movement_CurrencyId",
                table: "Movement",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyConversion_CurrencyId",
                table: "CurrencyConversion",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyConversion_MovementId",
                table: "CurrencyConversion",
                column: "MovementId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movement_Currency_CurrencyId",
                table: "Movement",
                column: "CurrencyId",
                principalTable: "Currency",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movement_Currency_CurrencyId",
                table: "Movement");

            migrationBuilder.DropTable(
                name: "CurrencyConversion");

            migrationBuilder.DropIndex(
                name: "IX_Movement_CurrencyId",
                table: "Movement");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ModuleEntry",
                table: "ModuleEntry");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Movement");

            migrationBuilder.DropColumn(
                name: "ShortName",
                table: "Currency");

            migrationBuilder.RenameTable(
                name: "ModuleEntry",
                newName: "Fund");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Fund",
                table: "Fund",
                column: "Id");
        }
    }
}
