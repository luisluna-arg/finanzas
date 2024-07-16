using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceApi.Domain.Migrations
{
    /// <inheritdoc />
    public partial class DebitsFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountDollars",
                table: "Debit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountDollars",
                table: "Debit",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
