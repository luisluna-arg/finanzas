using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class DebitOriginChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Debit_DebitOrigin_DebitOriginId",
                table: "Debit");

            migrationBuilder.RenameColumn(
                name: "DebitOriginId",
                table: "Debit",
                newName: "OriginId");

            migrationBuilder.RenameIndex(
                name: "IX_Debit_DebitOriginId",
                table: "Debit",
                newName: "IX_Debit_OriginId");

            migrationBuilder.CreateIndex(
                name: "IX_DebitOrigin_Name",
                table: "DebitOrigin",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Debit_DebitOrigin_OriginId",
                table: "Debit",
                column: "OriginId",
                principalTable: "DebitOrigin",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Debit_DebitOrigin_OriginId",
                table: "Debit");

            migrationBuilder.DropIndex(
                name: "IX_DebitOrigin_Name",
                table: "DebitOrigin");

            migrationBuilder.RenameColumn(
                name: "OriginId",
                table: "Debit",
                newName: "DebitOriginId");

            migrationBuilder.RenameIndex(
                name: "IX_Debit_OriginId",
                table: "Debit",
                newName: "IX_Debit_DebitOriginId");

            migrationBuilder.AddForeignKey(
                name: "FK_Debit_DebitOrigin_DebitOriginId",
                table: "Debit",
                column: "DebitOriginId",
                principalTable: "DebitOrigin",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
