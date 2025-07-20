using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIOLInvestmentAssetTypeResource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IOLInvestmentAssetTypeResource");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IOLInvestmentAssetTypeResource",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResourceSourceId = table.Column<short>(type: "smallint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IOLInvestmentAssetTypeResource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IOLInvestmentAssetTypeResource_IOLInvestmentAssetType_Resou~",
                        column: x => x.ResourceSourceId,
                        principalTable: "IOLInvestmentAssetType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IOLInvestmentAssetTypeResource_Resource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IOLInvestmentAssetTypeResource_ResourceId",
                table: "IOLInvestmentAssetTypeResource",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_IOLInvestmentAssetTypeResource_ResourceSourceId",
                table: "IOLInvestmentAssetTypeResource",
                column: "ResourceSourceId");
        }
    }
}
