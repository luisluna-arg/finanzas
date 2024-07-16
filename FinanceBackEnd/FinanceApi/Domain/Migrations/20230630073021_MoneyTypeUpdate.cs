using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FinanceApi.Domain.Migrations
{
    /// <inheritdoc />
    public partial class MoneyTypeUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "InvestmentAssetIOLTypes",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "InvestmentAssetIOLTypes",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "InvestmentAssetIOLTypes",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "InvestmentAssetIOLTypes",
                keyColumn: "Id",
                keyValue: 4L);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "InvestmentAssetIOLTypes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AlterColumn<int>(
                name: "InvestmentAssetIOLTypeId",
                table: "InvestmentAssetIOLs",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.InsertData(
                table: "InvestmentAssetIOLTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Cedear" },
                    { 2, "TituloPublico" },
                    { 3, "FCI" },
                    { 4, "ObligacionNegociable" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "InvestmentAssetIOLTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "InvestmentAssetIOLTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "InvestmentAssetIOLTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "InvestmentAssetIOLTypes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "InvestmentAssetIOLTypes",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AlterColumn<long>(
                name: "InvestmentAssetIOLTypeId",
                table: "InvestmentAssetIOLs",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.InsertData(
                table: "InvestmentAssetIOLTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1L, "Cedear" },
                    { 2L, "TituloPublico" },
                    { 3L, "FCI" },
                    { 4L, "ObligacionNegociable" }
                });
        }
    }
}
