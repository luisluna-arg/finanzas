using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class ModelsUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movement_Module_ModuleId",
                table: "Movement");

            migrationBuilder.DropTable(
                name: "Module");

            migrationBuilder.DropTable(
                name: "ModuleEntry");

            migrationBuilder.DropIndex(
                name: "IX_Movement_ModuleId",
                table: "Movement");

            migrationBuilder.AddColumn<Guid>(
                name: "AppModuleId",
                table: "Movement",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.CreateTable(
                name: "AppModule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppModule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppModule_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppModuleEntry",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Ammount = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppModuleEntry", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvestmentAssetIOLTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
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
                    Asset = table.Column<string>(type: "text", nullable: false),
                    Alarms = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<long>(type: "bigint", nullable: false),
                    Assets = table.Column<long>(type: "bigint", nullable: false),
                    DailyVariation = table.Column<decimal>(type: "numeric", nullable: false),
                    LastPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    AverageBuyPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    AverageReturnPercent = table.Column<decimal>(type: "numeric", nullable: false),
                    AverageReturn = table.Column<decimal>(type: "numeric", nullable: false),
                    Valued = table.Column<decimal>(type: "numeric", nullable: false),
                    InvestmentAssetIOLTypeId = table.Column<long>(type: "bigint", nullable: false)
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
                    { 1L, "Cedear" },
                    { 2L, "TituloPublico" },
                    { 3L, "FCI" },
                    { 4L, "ObligacionNegociable" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movement_AppModuleId",
                table: "Movement",
                column: "AppModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_AppModule_CurrencyId",
                table: "AppModule",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentAssetIOLs_InvestmentAssetIOLTypeId",
                table: "InvestmentAssetIOLs",
                column: "InvestmentAssetIOLTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movement_AppModule_AppModuleId",
                table: "Movement",
                column: "AppModuleId",
                principalTable: "AppModule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movement_AppModule_AppModuleId",
                table: "Movement");

            migrationBuilder.DropTable(
                name: "AppModule");

            migrationBuilder.DropTable(
                name: "AppModuleEntry");

            migrationBuilder.DropTable(
                name: "InvestmentAssetIOLs");

            migrationBuilder.DropTable(
                name: "InvestmentAssetIOLTypes");

            migrationBuilder.DropIndex(
                name: "IX_Movement_AppModuleId",
                table: "Movement");

            migrationBuilder.DropColumn(
                name: "AppModuleId",
                table: "Movement");

            migrationBuilder.CreateTable(
                name: "Module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Module", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Module_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModuleEntry",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Ammount = table.Column<decimal>(type: "numeric", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleEntry", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movement_ModuleId",
                table: "Movement",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Module_CurrencyId",
                table: "Module",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movement_Module_ModuleId",
                table: "Movement",
                column: "ModuleId",
                principalTable: "Module",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
