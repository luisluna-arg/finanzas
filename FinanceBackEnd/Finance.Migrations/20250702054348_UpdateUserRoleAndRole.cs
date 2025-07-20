using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserRoleAndRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DebitOriginResource",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResourceSourceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebitOriginResource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DebitOriginResource_DebitOrigin_ResourceSourceId",
                        column: x => x.ResourceSourceId,
                        principalTable: "DebitOrigin",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DebitOriginResource_Resource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IncomeResource",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResourceSourceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomeResource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncomeResource_Income_ResourceSourceId",
                        column: x => x.ResourceSourceId,
                        principalTable: "Income",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncomeResource_Resource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IOLInvestmentAssetResource",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResourceSourceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IOLInvestmentAssetResource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IOLInvestmentAssetResource_IOLInvestmentAsset_ResourceSourc~",
                        column: x => x.ResourceSourceId,
                        principalTable: "IOLInvestmentAsset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IOLInvestmentAssetResource_Resource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IOLInvestmentResource",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResourceSourceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IOLInvestmentResource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IOLInvestmentResource_IOLInvestment_ResourceSourceId",
                        column: x => x.ResourceSourceId,
                        principalTable: "IOLInvestment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IOLInvestmentResource_Resource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MovementResource",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResourceSourceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovementResource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovementResource_Movement_ResourceSourceId",
                        column: x => x.ResourceSourceId,
                        principalTable: "Movement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovementResource_Resource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<short>(type: "smallint", nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRole_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRole_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: ["Id", "CreatedAt", "Deactivated", "Name", "UpdatedAt"],
                values: [(short)1, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Admin", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)]);

            migrationBuilder.CreateIndex(
                name: "IX_DebitOriginResource_ResourceId",
                table: "DebitOriginResource",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_DebitOriginResource_ResourceSourceId",
                table: "DebitOriginResource",
                column: "ResourceSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeResource_ResourceId",
                table: "IncomeResource",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeResource_ResourceSourceId",
                table: "IncomeResource",
                column: "ResourceSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_IOLInvestmentAssetResource_ResourceId",
                table: "IOLInvestmentAssetResource",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_IOLInvestmentAssetResource_ResourceSourceId",
                table: "IOLInvestmentAssetResource",
                column: "ResourceSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_IOLInvestmentResource_ResourceId",
                table: "IOLInvestmentResource",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_IOLInvestmentResource_ResourceSourceId",
                table: "IOLInvestmentResource",
                column: "ResourceSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_MovementResource_ResourceId",
                table: "MovementResource",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_MovementResource_ResourceSourceId",
                table: "MovementResource",
                column: "ResourceSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleId",
                table: "UserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_UserId",
                table: "UserRole",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DebitOriginResource");

            migrationBuilder.DropTable(
                name: "IncomeResource");

            migrationBuilder.DropTable(
                name: "IOLInvestmentAssetResource");

            migrationBuilder.DropTable(
                name: "IOLInvestmentResource");

            migrationBuilder.DropTable(
                name: "MovementResource");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
