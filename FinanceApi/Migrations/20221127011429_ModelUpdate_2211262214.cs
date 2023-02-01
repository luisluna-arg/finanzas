using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceApi.Migrations
{
    /// <inheritdoc />
    public partial class ModelUpdate2211262214 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Concept1",
                table: "Movement",
                type: "text",
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<string>(
                name: "Concept2",
                table: "Movement",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "Movement",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Concept1",
                table: "Movement");

            migrationBuilder.DropColumn(
                name: "Concept2",
                table: "Movement");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "Movement");
        }
    }
}
