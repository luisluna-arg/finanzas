using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class FixingUnusedCreditCardFKs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditCard_CreditCardIssuer_CreditCardIssuerId1",
                table: "CreditCard");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCard_CreditCardStatement_CurrentStatementId1",
                table: "CreditCard");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardPayment_CreditCard_CreditCardId",
                table: "CreditCardPayment");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardStatementTransaction_CreditCard_CreditCardId",
                table: "CreditCardStatementTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardTransaction_CreditCardStatementTransaction_Statem~",
                table: "CreditCardTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardTransaction_CreditCard_CreditCardId1",
                table: "CreditCardTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCard_Bank_BankId1",
                table: "CreditCard");

            migrationBuilder.DropIndex(
                name: "IX_CreditCardTransaction_CreditCardId1",
                table: "CreditCardTransaction");

            migrationBuilder.DropIndex(
                name: "IX_CreditCardTransaction_StatementTransactionId1",
                table: "CreditCardTransaction");

            migrationBuilder.DropIndex(
                name: "IX_CreditCardStatementTransaction_CreditCardId",
                table: "CreditCardStatementTransaction");

            migrationBuilder.DropIndex(
                name: "IX_CreditCardPayment_CreditCardId_Timestamp",
                table: "CreditCardPayment");

            migrationBuilder.DropIndex(
                name: "IX_CreditCardPayment_StatementId",
                table: "CreditCardPayment");

            migrationBuilder.DropIndex(
                name: "IX_CreditCard_CreditCardIssuerId1",
                table: "CreditCard");

            migrationBuilder.DropIndex(
                name: "IX_CreditCard_CurrentStatementId1",
                table: "CreditCard");

            migrationBuilder.DropIndex(
                name: "IX_CreditCard_BankId1",
                table: "CreditCard");

            migrationBuilder.DropColumn(
                name: "CreditCardId1",
                table: "CreditCardTransaction");

            migrationBuilder.DropColumn(
                name: "StatementTransactionId1",
                table: "CreditCardTransaction");

            migrationBuilder.DropColumn(
                name: "CreditCardId",
                table: "CreditCardStatementTransaction");

            migrationBuilder.DropColumn(
                name: "CreditCardId",
                table: "CreditCardPayment");

            migrationBuilder.DropColumn(
                name: "CreditCardIssuerId1",
                table: "CreditCard");

            migrationBuilder.DropColumn(
                name: "CurrentStatementId1",
                table: "CreditCard");

            migrationBuilder.DropColumn(
                name: "BankId1",
                table: "CreditCard");

            migrationBuilder.AlterColumn<Guid>(
                name: "StatementId",
                table: "CreditCardPayment",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardTransaction_StatementTransactionId",
                table: "CreditCardTransaction",
                column: "StatementTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardPayment_StatementId_Timestamp",
                table: "CreditCardPayment",
                columns: ["StatementId", "Timestamp"]);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCard_CurrentStatementId",
                table: "CreditCard",
                column: "CurrentStatementId");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCard_CreditCardStatement_CurrentStatementId",
                table: "CreditCard",
                column: "CurrentStatementId",
                principalTable: "CreditCardStatement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardTransaction_CreditCardStatementTransaction_Statem~",
                table: "CreditCardTransaction",
                column: "StatementTransactionId",
                principalTable: "CreditCardStatementTransaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditCard_CreditCardStatement_CurrentStatementId",
                table: "CreditCard");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardTransaction_CreditCardStatementTransaction_Statem~",
                table: "CreditCardTransaction");

            migrationBuilder.DropIndex(
                name: "IX_CreditCardTransaction_StatementTransactionId",
                table: "CreditCardTransaction");

            migrationBuilder.DropIndex(
                name: "IX_CreditCardPayment_StatementId_Timestamp",
                table: "CreditCardPayment");

            migrationBuilder.DropIndex(
                name: "IX_CreditCard_CurrentStatementId",
                table: "CreditCard");

            migrationBuilder.AddColumn<Guid>(
                name: "CreditCardId1",
                table: "CreditCardTransaction",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StatementTransactionId1",
                table: "CreditCardTransaction",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreditCardId",
                table: "CreditCardStatementTransaction",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "StatementId",
                table: "CreditCardPayment",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "CreditCardId",
                table: "CreditCardPayment",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CreditCardIssuerId1",
                table: "CreditCard",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CurrentStatementId1",
                table: "CreditCard",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BankId1",
                table: "CreditCard",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardTransaction_CreditCardId1",
                table: "CreditCardTransaction",
                column: "CreditCardId1");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardTransaction_StatementTransactionId1",
                table: "CreditCardTransaction",
                column: "StatementTransactionId1");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardStatementTransaction_CreditCardId",
                table: "CreditCardStatementTransaction",
                column: "CreditCardId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardPayment_CreditCardId_Timestamp",
                table: "CreditCardPayment",
                columns: ["CreditCardId", "Timestamp"]);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardPayment_StatementId",
                table: "CreditCardPayment",
                column: "StatementId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCard_CreditCardIssuerId1",
                table: "CreditCard",
                column: "CreditCardIssuerId1");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCard_CurrentStatementId1",
                table: "CreditCard",
                column: "CurrentStatementId1");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCard_BankId1",
                table: "CreditCard",
                column: "BankId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCard_CreditCardIssuer_CreditCardIssuerId1",
                table: "CreditCard",
                column: "CreditCardIssuerId1",
                principalTable: "CreditCardIssuer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCard_CreditCardStatement_CurrentStatementId1",
                table: "CreditCard",
                column: "CurrentStatementId1",
                principalTable: "CreditCardStatement",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardPayment_CreditCard_CreditCardId",
                table: "CreditCardPayment",
                column: "CreditCardId",
                principalTable: "CreditCard",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardStatementTransaction_CreditCard_CreditCardId",
                table: "CreditCardStatementTransaction",
                column: "CreditCardId",
                principalTable: "CreditCard",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardTransaction_CreditCardStatementTransaction_Statem~",
                table: "CreditCardTransaction",
                column: "StatementTransactionId1",
                principalTable: "CreditCardStatementTransaction",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardTransaction_CreditCard_CreditCardId1",
                table: "CreditCardTransaction",
                column: "CreditCardId1",
                principalTable: "CreditCard",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCard_Bank_BankId1",
                table: "CreditCard",
                column: "BankId1",
                principalTable: "Bank",
                principalColumn: "Id");
        }
    }
}
