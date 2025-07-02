using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class ResourceUserOwnership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Movement",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "IOLInvestment",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Income",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Fund",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "CurrencyExchangeRate",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "CurrencyExchangeRate",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "AppModule",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "IdentityProvider",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityProvider", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Resource",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resource", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CreditCardMovementResource",
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
                    table.PrimaryKey("PK_CreditCardMovementResource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditCardMovementResource_CreditCardMovement_ResourceSourc~",
                        column: x => x.ResourceSourceId,
                        principalTable: "CreditCardMovement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreditCardMovementResource_Resource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreditCardResource",
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
                    table.PrimaryKey("PK_CreditCardResource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditCardResource_CreditCard_ResourceSourceId",
                        column: x => x.ResourceSourceId,
                        principalTable: "CreditCard",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreditCardResource_Resource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreditCardStatementResource",
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
                    table.PrimaryKey("PK_CreditCardStatementResource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditCardStatementResource_CreditCardStatement_ResourceSou~",
                        column: x => x.ResourceSourceId,
                        principalTable: "CreditCardStatement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreditCardStatementResource_Resource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CurrencyExchangeRateResource",
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
                    table.PrimaryKey("PK_CurrencyExchangeRateResource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurrencyExchangeRateResource_CurrencyExchangeRate_ResourceS~",
                        column: x => x.ResourceSourceId,
                        principalTable: "CurrencyExchangeRate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CurrencyExchangeRateResource_Resource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DebitResource",
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
                    table.PrimaryKey("PK_DebitResource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DebitResource_Debit_ResourceSourceId",
                        column: x => x.ResourceSourceId,
                        principalTable: "Debit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DebitResource_Resource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FundResource",
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
                    table.PrimaryKey("PK_FundResource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FundResource_Fund_ResourceSourceId",
                        column: x => x.ResourceSourceId,
                        principalTable: "Fund",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FundResource_Resource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IOLInvestmentAssetTypeResource",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResourceSourceId1 = table.Column<int>(type: "integer", nullable: false),
                    ResourceSourceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IOLInvestmentAssetTypeResource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IOLInvestmentAssetTypeResource_IOLInvestmentAssetType_Resou~",
                        column: x => x.ResourceSourceId1,
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

            migrationBuilder.CreateTable(
                name: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Provider = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UserId1 = table.Column<Guid>(type: "uuid", nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Identity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Identity_User_UserId1",
                        column: x => x.UserId1,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResourceOwner",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceOwner", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceOwner_Resource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceOwner_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "IdentityProvider",
                columns: ["Id", "CreatedAt", "Deactivated", "Name", "UpdatedAt"],
                values: [(short)1, new DateTime(1, 1, 1, 3, 0, 0, 0, DateTimeKind.Utc), false, "Auth", new DateTime(1, 1, 1, 3, 0, 0, 0, DateTimeKind.Utc)]);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardMovementResource_ResourceId",
                table: "CreditCardMovementResource",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardMovementResource_ResourceSourceId",
                table: "CreditCardMovementResource",
                column: "ResourceSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardResource_ResourceId",
                table: "CreditCardResource",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardResource_ResourceSourceId",
                table: "CreditCardResource",
                column: "ResourceSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardStatementResource_ResourceId",
                table: "CreditCardStatementResource",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardStatementResource_ResourceSourceId",
                table: "CreditCardStatementResource",
                column: "ResourceSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRateResource_ResourceId",
                table: "CurrencyExchangeRateResource",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRateResource_ResourceSourceId",
                table: "CurrencyExchangeRateResource",
                column: "ResourceSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_DebitResource_ResourceId",
                table: "DebitResource",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_DebitResource_ResourceSourceId",
                table: "DebitResource",
                column: "ResourceSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_FundResource_ResourceId",
                table: "FundResource",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_FundResource_ResourceSourceId",
                table: "FundResource",
                column: "ResourceSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Identity_UserId1",
                table: "Identity",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_IOLInvestmentAssetTypeResource_ResourceId",
                table: "IOLInvestmentAssetTypeResource",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_IOLInvestmentAssetTypeResource_ResourceSourceId1",
                table: "IOLInvestmentAssetTypeResource",
                column: "ResourceSourceId1");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceOwner_ResourceId",
                table: "ResourceOwner",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceOwner_UserId",
                table: "ResourceOwner",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreditCardMovementResource");

            migrationBuilder.DropTable(
                name: "CreditCardResource");

            migrationBuilder.DropTable(
                name: "CreditCardStatementResource");

            migrationBuilder.DropTable(
                name: "CurrencyExchangeRateResource");

            migrationBuilder.DropTable(
                name: "DebitResource");

            migrationBuilder.DropTable(
                name: "FundResource");

            migrationBuilder.DropTable(
                name: "Identity");

            migrationBuilder.DropTable(
                name: "IdentityProvider");

            migrationBuilder.DropTable(
                name: "IOLInvestmentAssetTypeResource");

            migrationBuilder.DropTable(
                name: "ResourceOwner");

            migrationBuilder.DropTable(
                name: "Resource");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Movement");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "IOLInvestment");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Income");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Fund");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CurrencyExchangeRate");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "CurrencyExchangeRate");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AppModule");
        }
    }
}
