using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFrequencyTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var date = new DateTime(2025, 11, 27, 0, 0, 0, 0, DateTimeKind.Utc);

            migrationBuilder.InsertData(
                table: "Frequency",
                columns: ["Id", "CreatedAt", "Deactivated", "Name", "UpdatedAt"],
                values: new object[,]
                {
                    { (short)2, date, false, "Weekly", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { (short)3, date, false, "Daily", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { (short)4, date, false, "OneTime", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Frequency",
                keyColumn: "Id",
                keyValue: (short)2);

            migrationBuilder.DeleteData(
                table: "Frequency",
                keyColumn: "Id",
                keyValue: (short)3);

            migrationBuilder.DeleteData(
                table: "Frequency",
                keyColumn: "Id",
                keyValue: (short)4);
        }
    }
}
