using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class RenameCreditCardStatementNavigationProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardStatementAdjustment_CreditCardStatement_CreditCar~",
                table: "CreditCardStatementAdjustment");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardStatementTransaction_CreditCardStatement_CreditCa~",
                table: "CreditCardStatementTransaction");

            migrationBuilder.RenameColumn(
                name: "CreditCardStatementId",
                table: "CreditCardStatementTransaction",
                newName: "StatementId");

            migrationBuilder.RenameIndex(
                name: "IX_CreditCardStatementTransaction_CreditCardStatementId_Posted~",
                table: "CreditCardStatementTransaction",
                newName: "IX_CreditCardStatementTransaction_StatementId_PostedDate");

            migrationBuilder.RenameColumn(
                name: "CreditCardStatementId",
                table: "CreditCardStatementAdjustment",
                newName: "StatementId");

            migrationBuilder.RenameIndex(
                name: "IX_CreditCardStatementAdjustment_CreditCardStatementId_Created~",
                table: "CreditCardStatementAdjustment",
                newName: "IX_CreditCardStatementAdjustment_StatementId_CreatedAt");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardStatementAdjustment_CreditCardStatement_Statement~",
                table: "CreditCardStatementAdjustment",
                column: "StatementId",
                principalTable: "CreditCardStatement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardStatementTransaction_CreditCardStatement_Statemen~",
                table: "CreditCardStatementTransaction",
                column: "StatementId",
                principalTable: "CreditCardStatement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardStatementAdjustment_CreditCardStatement_Statement~",
                table: "CreditCardStatementAdjustment");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardStatementTransaction_CreditCardStatement_Statemen~",
                table: "CreditCardStatementTransaction");

            migrationBuilder.RenameColumn(
                name: "StatementId",
                table: "CreditCardStatementTransaction",
                newName: "CreditCardStatementId");

            migrationBuilder.RenameIndex(
                name: "IX_CreditCardStatementTransaction_StatementId_PostedDate",
                table: "CreditCardStatementTransaction",
                newName: "IX_CreditCardStatementTransaction_CreditCardStatementId_Posted~");

            migrationBuilder.RenameColumn(
                name: "StatementId",
                table: "CreditCardStatementAdjustment",
                newName: "CreditCardStatementId");

            migrationBuilder.RenameIndex(
                name: "IX_CreditCardStatementAdjustment_StatementId_CreatedAt",
                table: "CreditCardStatementAdjustment",
                newName: "IX_CreditCardStatementAdjustment_CreditCardStatementId_Created~");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardStatementAdjustment_CreditCardStatement_CreditCar~",
                table: "CreditCardStatementAdjustment",
                column: "CreditCardStatementId",
                principalTable: "CreditCardStatement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardStatementTransaction_CreditCardStatement_CreditCa~",
                table: "CreditCardStatementTransaction",
                column: "CreditCardStatementId",
                principalTable: "CreditCardStatement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
