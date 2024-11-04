using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class MovementBankRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BankId",
                table: "Movement",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Movement_BankId",
                table: "Movement",
                column: "BankId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movement_Bank_BankId",
                table: "Movement",
                column: "BankId",
                principalTable: "Bank",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movement_Bank_BankId",
                table: "Movement");

            migrationBuilder.DropIndex(
                name: "IX_Movement_BankId",
                table: "Movement");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "Movement");
        }
    }
}
