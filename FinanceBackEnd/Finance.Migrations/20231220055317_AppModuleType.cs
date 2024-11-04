using Finance.Domain.Enums;
using Finance.Persistance;
using Finance.Persistance.Constants;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AppModuleType : Migration
    {
        private const string AppModuleTable = "AppModule";

        private const string AppModuleTypeTable = "AppModuleType";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "TypeId",
                table: AppModuleTable,
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            foreach (var item in AppModuleConstants.Types)
            {
                migrationBuilder.Sql($"UPDATE public.\"{AppModuleTable}\" SET \"TypeId\" = {(short)item.Value} WHERE \"Id\" = '{item.Key}'");
            }

            migrationBuilder.CreateTable(
                name: AppModuleTypeTable,
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppModuleType", x => x.Id);
                });

            AppModuleTypeEnum[] enumValues = (AppModuleTypeEnum[])Enum.GetValues(typeof(AppModuleTypeEnum));
            foreach (var enumValue in enumValues)
            {
                migrationBuilder.InsertData(
                    table: AppModuleTypeTable,
                    columns: new[] { "Id", "Name", "Deactivated" },
                    values: new object[] { (short)enumValue, $"{enumValue}", false });
            }

            migrationBuilder.CreateIndex(
                name: "IX_AppModule_TypeId",
                table: AppModuleTable,
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppModule_AppModuleType_TypeId",
                table: AppModuleTable,
                column: "TypeId",
                principalTable: "AppModuleType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppModule_AppModuleType_TypeId",
                table: AppModuleTable);

            migrationBuilder.DropTable(
                name: AppModuleTypeTable);

            migrationBuilder.DropIndex(
                name: "IX_AppModule_TypeId",
                table: AppModuleTable);

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: AppModuleTable);
        }
    }
}
