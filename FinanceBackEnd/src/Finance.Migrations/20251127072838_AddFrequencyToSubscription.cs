using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddFrequencyToSubscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "Frequency",
                table: "Subscriptions",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_Frequency",
                table: "Subscriptions",
                column: "Frequency");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Frequency_Frequency",
                table: "Subscriptions",
                column: "Frequency",
                principalTable: "Frequency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Frequency_Frequency",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_Frequency",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "Subscriptions");
        }
    }
}
