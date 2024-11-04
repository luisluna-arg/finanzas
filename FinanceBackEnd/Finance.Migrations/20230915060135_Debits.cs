using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class Debits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DebitOrigin",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AppModuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebitOrigin", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DebitOrigin_AppModule_AppModuleId",
                        column: x => x.AppModuleId,
                        principalTable: "AppModule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Debit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DebitOriginId = table.Column<Guid>(type: "uuid", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    AmountDollars = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Debit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Debit_DebitOrigin_DebitOriginId",
                        column: x => x.DebitOriginId,
                        principalTable: "DebitOrigin",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Debit_DebitOriginId",
                table: "Debit",
                column: "DebitOriginId");

            migrationBuilder.CreateIndex(
                name: "IX_DebitOrigin_AppModuleId",
                table: "DebitOrigin",
                column: "AppModuleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Debit");

            migrationBuilder.DropTable(
                name: "DebitOrigin");
        }
    }
}
