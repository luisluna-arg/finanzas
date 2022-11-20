using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceApi.Migrations
{
    /// <inheritdoc />
    public partial class Update2211202015 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modules_Currencies_CurrencyId",
                table: "Modules");

            migrationBuilder.DropForeignKey(
                name: "FK_Movements_Modules_ModuleId",
                table: "Movements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Movements",
                table: "Movements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Modules",
                table: "Modules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Funds",
                table: "Funds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Currencies",
                table: "Currencies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Banks",
                table: "Banks");

            migrationBuilder.RenameTable(
                name: "Movements",
                newName: "Movement");

            migrationBuilder.RenameTable(
                name: "Modules",
                newName: "Module");

            migrationBuilder.RenameTable(
                name: "Funds",
                newName: "Fund");

            migrationBuilder.RenameTable(
                name: "Currencies",
                newName: "Currency");

            migrationBuilder.RenameTable(
                name: "Banks",
                newName: "Bank");

            migrationBuilder.RenameIndex(
                name: "IX_Movements_ModuleId",
                table: "Movement",
                newName: "IX_Movement_ModuleId");

            migrationBuilder.RenameIndex(
                name: "IX_Modules_CurrencyId",
                table: "Module",
                newName: "IX_Module_CurrencyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Movement",
                table: "Movement",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Module",
                table: "Module",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Fund",
                table: "Fund",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Currency",
                table: "Currency",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bank",
                table: "Bank",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Module_Currency_CurrencyId",
                table: "Module",
                column: "CurrencyId",
                principalTable: "Currency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Movement_Module_ModuleId",
                table: "Movement",
                column: "ModuleId",
                principalTable: "Module",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Module_Currency_CurrencyId",
                table: "Module");

            migrationBuilder.DropForeignKey(
                name: "FK_Movement_Module_ModuleId",
                table: "Movement");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Movement",
                table: "Movement");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Module",
                table: "Module");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Fund",
                table: "Fund");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Currency",
                table: "Currency");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bank",
                table: "Bank");

            migrationBuilder.RenameTable(
                name: "Movement",
                newName: "Movements");

            migrationBuilder.RenameTable(
                name: "Module",
                newName: "Modules");

            migrationBuilder.RenameTable(
                name: "Fund",
                newName: "Funds");

            migrationBuilder.RenameTable(
                name: "Currency",
                newName: "Currencies");

            migrationBuilder.RenameTable(
                name: "Bank",
                newName: "Banks");

            migrationBuilder.RenameIndex(
                name: "IX_Movement_ModuleId",
                table: "Movements",
                newName: "IX_Movements_ModuleId");

            migrationBuilder.RenameIndex(
                name: "IX_Module_CurrencyId",
                table: "Modules",
                newName: "IX_Modules_CurrencyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Movements",
                table: "Movements",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Modules",
                table: "Modules",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Funds",
                table: "Funds",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Currencies",
                table: "Currencies",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Banks",
                table: "Banks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Modules_Currencies_CurrencyId",
                table: "Modules",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Movements_Modules_ModuleId",
                table: "Movements",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
