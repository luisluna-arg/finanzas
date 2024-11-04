using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class IOLInvestmentsRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppModuleEntry");

            migrationBuilder.DropTable(
                name: "InvestmentAssetIOLs");

            migrationBuilder.DropTable(
                name: "InvestmentAssetIOLTypes");

            migrationBuilder.CreateTable(
                name: "IOLInvestmentAssetTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IOLInvestmentAssetTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IOLInvestmentAssets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Symbol = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    TypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IOLInvestmentAssets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IOLInvestmentAssets_IOLInvestmentAssetTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "IOLInvestmentAssetTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IOLInvestments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Alarms = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<long>(type: "bigint", nullable: false),
                    Assets = table.Column<long>(type: "bigint", nullable: false),
                    DailyVariation = table.Column<decimal>(type: "numeric", nullable: false),
                    LastPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    AverageBuyPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    AverageReturnPercent = table.Column<decimal>(type: "numeric", nullable: false),
                    AverageReturn = table.Column<decimal>(type: "numeric", nullable: false),
                    Valued = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IOLInvestments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IOLInvestments_IOLInvestmentAssets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "IOLInvestmentAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "IOLInvestmentAssetTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Cedear" },
                    { 2, "TituloPublico" },
                    { 3, "FCI" },
                    { 4, "ObligacionNegociable" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_IOLInvestmentAssets_TypeId",
                table: "IOLInvestmentAssets",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_IOLInvestments_AssetId",
                table: "IOLInvestments",
                column: "AssetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IOLInvestments");

            migrationBuilder.DropTable(
                name: "IOLInvestmentAssets");

            migrationBuilder.DropTable(
                name: "IOLInvestmentAssetTypes");

            migrationBuilder.CreateTable(
                name: "AppModuleEntry",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Ammount = table.Column<decimal>(type: "numeric", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppModuleEntry", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvestmentAssetIOLTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentAssetIOLTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvestmentAssetIOLs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InvestmentAssetIOLTypeId = table.Column<int>(type: "integer", nullable: false),
                    Alarms = table.Column<long>(type: "bigint", nullable: false),
                    Asset = table.Column<string>(type: "text", nullable: false),
                    Assets = table.Column<long>(type: "bigint", nullable: false),
                    AverageBuyPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    AverageReturn = table.Column<decimal>(type: "numeric", nullable: false),
                    AverageReturnPercent = table.Column<decimal>(type: "numeric", nullable: false),
                    DailyVariation = table.Column<decimal>(type: "numeric", nullable: false),
                    LastPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    Quantity = table.Column<long>(type: "bigint", nullable: false),
                    Valued = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentAssetIOLs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvestmentAssetIOLs_InvestmentAssetIOLTypes_InvestmentAsset~",
                        column: x => x.InvestmentAssetIOLTypeId,
                        principalTable: "InvestmentAssetIOLTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentAssetIOLs_InvestmentAssetIOLTypeId",
                table: "InvestmentAssetIOLs",
                column: "InvestmentAssetIOLTypeId");
        }
    }
}
