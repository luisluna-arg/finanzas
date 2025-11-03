using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class KeyValueEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "IOLInvestmentAssetType",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "IOLInvestmentAssetType",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "IOLInvestmentAssetType",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "IOLInvestmentAssetType",
                keyColumn: "Id",
                keyValue: 4);

            // Drop sequence if it exists before altering the column
            migrationBuilder.Sql(@"DROP SEQUENCE IF EXISTS ""IOLInvestmentAssetType_Id_seq"" CASCADE;");

            migrationBuilder.AlterColumn<short>(
                name: "ResourceSourceId1",
                table: "IOLInvestmentAssetTypeResource",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "IOLInvestmentAssetTypeResource",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "IOLInvestmentAssetType",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<short>(
                name: "Id",
                table: "IOLInvestmentAssetType",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "IOLInvestmentAssetType",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "IOLInvestmentAssetType",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<short>(
                name: "TypeId",
                table: "IOLInvestmentAsset",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Frequency",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Frequency",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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

            migrationBuilder.InsertData(
                table: "IOLInvestmentAssetType",
                columns: ["Id", "CreatedAt", "Deactivated", "Name", "UpdatedAt"],
                values: new object[,]
                {
                    { (short)1, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Cedear", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { (short)2, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "TituloPublico", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { (short)3, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "FCI", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { (short)4, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "ObligacionNegociable", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "IOLInvestmentAssetType",
                keyColumn: "Id",
                keyValue: (short)1);

            migrationBuilder.DeleteData(
                table: "IOLInvestmentAssetType",
                keyColumn: "Id",
                keyValue: (short)2);

            migrationBuilder.DeleteData(
                table: "IOLInvestmentAssetType",
                keyColumn: "Id",
                keyValue: (short)3);

            migrationBuilder.DeleteData(
                table: "IOLInvestmentAssetType",
                keyColumn: "Id",
                keyValue: (short)4);

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "IOLInvestmentAssetType");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "IOLInvestmentAssetType");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Frequency");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Frequency");

            migrationBuilder.AlterColumn<int>(
                name: "ResourceSourceId1",
                table: "IOLInvestmentAssetTypeResource",
                type: "integer",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "IOLInvestmentAssetTypeResource",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "IOLInvestmentAssetType",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "IOLInvestmentAssetType",
                type: "integer",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AlterColumn<int>(
                name: "TypeId",
                table: "IOLInvestmentAsset",
                type: "integer",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.InsertData(
                table: "IOLInvestmentAssetType",
                columns: ["Id", "Deactivated", "Name"],
                values: new object[,]
                {
                    { 1, false, "Cedear" },
                    { 2, false, "TituloPublico" },
                    { 3, false, "FCI" },
                    { 4, false, "ObligacionNegociable" }
                });
        }
    }
}
