using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class FixUserRoleRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRole",
                table: "UserRole");

            migrationBuilder.DropIndex(
                name: "IX_UserRole_UserId",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "Deactivated",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "UserRole");

            // Remove duplicate UserRole relations before adding composite PK
            migrationBuilder.Sql(@"
                DELETE FROM ""UserRole"" a
                USING ""UserRole"" b
                WHERE a.""UserId"" = b.""UserId""
                  AND a.""RoleId"" = b.""RoleId""
                  AND a.ctid < b.ctid;
            ");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRole",
                table: "UserRole",
                columns: ["UserId", "RoleId"]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRole",
                table: "UserRole");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "UserRole",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "UserRole",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Deactivated",
                table: "UserRole",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "UserRole",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRole",
                table: "UserRole",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_UserId",
                table: "UserRole",
                column: "UserId");
        }
    }
}
