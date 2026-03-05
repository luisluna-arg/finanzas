using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class FixIdentityColumnNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the foreign key constraint for UserId1
            migrationBuilder.DropForeignKey(
                name: "FK_Identity_User_UserId1",
                table: "Identity");

            // Drop the index for UserId1
            migrationBuilder.DropIndex(
                name: "IX_Identity_UserId1",
                table: "Identity");

            // First: Rename UserId to SourceId (this contains the external IdP user ID)
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Identity",
                newName: "SourceId");

            // Second: Rename UserId1 to UserId (this becomes the foreign key)
            migrationBuilder.RenameColumn(
                name: "UserId1",
                table: "Identity",
                newName: "UserId");

            migrationBuilder.InsertData(
                table: "Role",
                columns: ["Id", "CreatedAt", "Deactivated", "Name", "UpdatedAt"],
                values: [(short)2, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Owner", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)]);

            // Create index for the new UserId foreign key
            migrationBuilder.CreateIndex(
                name: "IX_Identity_UserId",
                table: "Identity",
                column: "UserId");

            // Add the foreign key constraint for the new UserId
            migrationBuilder.AddForeignKey(
                name: "FK_Identity_User_UserId",
                table: "Identity",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the foreign key constraint for UserId
            migrationBuilder.DropForeignKey(
                name: "FK_Identity_User_UserId",
                table: "Identity");

            // Drop the index for UserId
            migrationBuilder.DropIndex(
                name: "IX_Identity_UserId",
                table: "Identity");

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Id",
                keyValue: (short)2);

            // First: Rename UserId back to UserId1
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Identity",
                newName: "UserId1");

            // Second: Rename SourceId back to UserId
            migrationBuilder.RenameColumn(
                name: "SourceId",
                table: "Identity",
                newName: "UserId");

            // Create index for UserId1
            migrationBuilder.CreateIndex(
                name: "IX_Identity_UserId1",
                table: "Identity",
                column: "UserId1");

            // Add the foreign key constraint for UserId1
            migrationBuilder.AddForeignKey(
                name: "FK_Identity_User_UserId1",
                table: "Identity",
                column: "UserId1",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
