using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceApi.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AppModuleRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModuleId",
                table: "Movement");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ModuleId",
                table: "Movement",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty);
        }
    }
}
