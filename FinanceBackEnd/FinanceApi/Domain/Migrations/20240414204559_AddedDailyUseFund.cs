using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceApi.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddedDailyUseFund : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DailyUse",
                table: "Fund",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DailyUse",
                table: "Fund");
        }
    }
}
