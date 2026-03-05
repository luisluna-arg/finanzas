using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class DebitOriginTableFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DebitOrigin_Name",
                table: "DebitOrigin");

            migrationBuilder.CreateIndex(
                name: "IX_DebitOrigin_Name_AppModuleId",
                table: "DebitOrigin",
                columns: ["Name", "AppModuleId"],
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DebitOrigin_Name_AppModuleId",
                table: "DebitOrigin");

            migrationBuilder.CreateIndex(
                name: "IX_DebitOrigin_Name",
                table: "DebitOrigin",
                column: "Name",
                unique: true);
        }
    }
}
