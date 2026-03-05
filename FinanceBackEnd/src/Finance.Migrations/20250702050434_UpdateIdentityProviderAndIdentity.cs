using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIdentityProviderAndIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<short>(
                name: "Provider",
                table: "Identity",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.UpdateData(
                table: "IdentityProvider",
                keyColumn: "Id",
                keyValue: (short)1,
                columns: ["CreatedAt", "UpdatedAt"],
                values: [new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)]);

            migrationBuilder.InsertData(
                table: "IdentityProvider",
                columns: ["Id", "CreatedAt", "Deactivated", "Name", "UpdatedAt"],
                values: [(short)0, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "None", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "IdentityProvider",
                keyColumn: "Id",
                keyValue: (short)0);

            migrationBuilder.AlterColumn<int>(
                name: "Provider",
                table: "Identity",
                type: "integer",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.UpdateData(
                table: "IdentityProvider",
                keyColumn: "Id",
                keyValue: (short)1,
                columns: ["CreatedAt", "UpdatedAt"],
                values: [new DateTime(1, 1, 1, 3, 0, 0, 0, DateTimeKind.Utc), new DateTime(1, 1, 1, 3, 0, 0, 0, DateTimeKind.Utc)]);
        }
    }
}
