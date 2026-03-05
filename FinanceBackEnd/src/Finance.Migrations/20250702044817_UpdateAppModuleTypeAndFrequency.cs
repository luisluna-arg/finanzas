using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAppModuleTypeAndFrequency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AppModuleType",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<short>(
                name: "Id",
                table: "AppModuleType",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AppModuleType",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "AppModuleType",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            // Update existing AppModuleType records to set audit fields
            migrationBuilder.Sql(@"
                UPDATE ""AppModuleType"" 
                SET ""CreatedAt"" = TIMESTAMPTZ '2023-01-01T00:00:00Z', 
                    ""UpdatedAt"" = TIMESTAMPTZ '2023-01-01T00:00:00Z';
            ");

            migrationBuilder.UpdateData(
                table: "Frequency",
                keyColumn: "Id",
                keyValue: (short)0,
                columns: ["CreatedAt", "UpdatedAt"],
                values: [new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)]);

            migrationBuilder.UpdateData(
                table: "Frequency",
                keyColumn: "Id",
                keyValue: (short)1,
                columns: ["CreatedAt", "UpdatedAt"],
                values: [new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AppModuleType");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AppModuleType");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AppModuleType",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<short>(
                name: "Id",
                table: "AppModuleType",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.UpdateData(
                table: "Frequency",
                keyColumn: "Id",
                keyValue: (short)0,
                columns: ["CreatedAt", "UpdatedAt"],
                values: [new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)]);

            migrationBuilder.UpdateData(
                table: "Frequency",
                keyColumn: "Id",
                keyValue: (short)1,
                columns: ["CreatedAt", "UpdatedAt"],
                values: [new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)]);
        }
    }
}
