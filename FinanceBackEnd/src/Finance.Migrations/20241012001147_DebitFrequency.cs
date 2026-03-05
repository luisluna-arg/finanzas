using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class DebitFrequency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "Frequency",
                table: "Debit",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateTable(
                name: "Frequency",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Frequency", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Frequency",
                columns: ["Id", "Deactivated", "Name"],
                values: new object[,]
                {
                    { (short)0, false, "Monthly" },
                    { (short)1, false, "Annual" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Debit_Frequency",
                table: "Debit",
                column: "Frequency");

            migrationBuilder.AddForeignKey(
                name: "FK_Debit_Frequency_Frequency",
                table: "Debit",
                column: "Frequency",
                principalTable: "Frequency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Debit_Frequency_Frequency",
                table: "Debit");

            migrationBuilder.DropTable(
                name: "Frequency");

            migrationBuilder.DropIndex(
                name: "IX_Debit_Frequency",
                table: "Debit");

            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "Debit");
        }
    }
}
