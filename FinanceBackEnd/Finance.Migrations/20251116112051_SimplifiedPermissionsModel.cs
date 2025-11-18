using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class SimplifiedPermissionsModel : Migration
    {
        /// <inheritdoc />
        /// <summary>
        /// UP: Migrate FROM Resource/ResourceOwner model TO simplified direct permissions model
        /// </summary>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First create the new permission tables and permission level lookup
            migrationBuilder.CreateTable(
                name: "PermissionLevel",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionLevel", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "PermissionLevel",
                columns: ["Id", "CreatedAt", "Deactivated", "Name", "UpdatedAt"],
                values: new object[,]
                {
                    { (short)0, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "None", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { (short)1, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Read", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { (short)2, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Write", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { (short)3, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Owner", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            // Create all permission tables first
            Up_CreatePermissionTable(migrationBuilder, "CreditCardPermissions", "CreditCard");
            Up_CreatePermissionTable(migrationBuilder, "CurrencyExchangeRatePermissions", "CurrencyExchangeRate", "FK_CurrencyExchangeRatePermissions_CurrencyExchangeRate_Resour~");
            Up_CreatePermissionTable(migrationBuilder, "DebitOriginPermissions", "DebitOrigin");
            Up_CreatePermissionTable(migrationBuilder, "DebitPermissions", "Debit");
            Up_CreatePermissionTable(migrationBuilder, "FundPermissions", "Fund");
            Up_CreatePermissionTable(migrationBuilder, "IncomePermissions", "Income");
            Up_CreatePermissionTable(migrationBuilder, "IOLInvestmentAssetPermissions", "IOLInvestmentAsset");
            Up_CreatePermissionTable(migrationBuilder, "IOLInvestmentPermissions", "IOLInvestment");
            Up_CreatePermissionTable(migrationBuilder, "MovementPermissions", "Movement");

            // Now migrate data from old tables to new permission tables before dropping them
            Up_MigrateDataToPermissionTables(migrationBuilder);

            // Now create indexes for the new permission tables
            Up_CreatePermissionIndex(migrationBuilder, "CreditCardPermissions");
            Up_CreatePermissionIndex(migrationBuilder, "CurrencyExchangeRatePermissions");
            Up_CreatePermissionIndex(migrationBuilder, "DebitOriginPermissions");
            Up_CreatePermissionIndex(migrationBuilder, "DebitPermissions");
            Up_CreatePermissionIndex(migrationBuilder, "FundPermissions");
            Up_CreatePermissionIndex(migrationBuilder, "IncomePermissions");
            Up_CreatePermissionIndex(migrationBuilder, "IOLInvestmentAssetPermissions");
            Up_CreatePermissionIndex(migrationBuilder, "IOLInvestmentPermissions");
            Up_CreatePermissionIndex(migrationBuilder, "MovementPermissions");

            // Finally drop the old tables after data migration
            Up_DropOldResourceTables(migrationBuilder);
        }

        /// <inheritdoc />
        /// <summary>
        /// DOWN: Migrate FROM simplified direct permissions model BACK TO Resource/ResourceOwner model
        /// </summary>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Recreate the base Resource table first
            Down_CreateResourceTable(migrationBuilder);

            // Create resource mapping tables
            Down_CreateResourceMappingTables(migrationBuilder);

            // Create ResourceOwner table
            Down_CreateResourceOwnerTable(migrationBuilder);

            // Create all indexes for the recreated tables
            Down_CreateResourceTableIndexes(migrationBuilder);

            // Migrate data back from permission tables to old structure before dropping them
            Down_MigrateDataFromPermissionTables(migrationBuilder);

            // Finally drop new permission tables after data migration
            Down_DropNewPermissionTables(migrationBuilder);
        }

        // Helper methods for Up method
        private void Up_CreatePermissionTable(MigrationBuilder migrationBuilder, string tableName, string resourceTable, string resourceForeignKeyName = null)
        {
            var fkName = resourceForeignKeyName ?? $"FK_{tableName}_{resourceTable}_ResourceId";

            migrationBuilder.CreateTable(
                name: tableName,
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionLevels = table.Column<int[]>(type: "integer[]", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey($"PK_{tableName}", x => x.Id);
                    table.ForeignKey(
                        name: fkName,
                        column: x => x.ResourceId,
                        principalTable: resourceTable,
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: $"FK_{tableName}_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        private void Up_CreatePermissionIndex(MigrationBuilder migrationBuilder, string tableName)
        {
            migrationBuilder.CreateIndex(
                name: $"IX_{tableName}_ResourceId_UserId",
                table: tableName,
                columns: ["ResourceId", "UserId"],
                unique: true);
        }

        private void Up_MigrateDataToPermissionTables(MigrationBuilder migrationBuilder)
        {
            var dataMigrations = new[]
            {
                new { PermissionTable = "CreditCardPermissions", ResourceTable = "CreditCardResource", Alias = "cr" },
                new { PermissionTable = "CurrencyExchangeRatePermissions", ResourceTable = "CurrencyExchangeRateResource", Alias = "cer" },
                new { PermissionTable = "DebitOriginPermissions", ResourceTable = "DebitOriginResource", Alias = "dor" },
                new { PermissionTable = "DebitPermissions", ResourceTable = "DebitResource", Alias = "dr" },
                new { PermissionTable = "FundPermissions", ResourceTable = "FundResource", Alias = "fr" },
                new { PermissionTable = "IncomePermissions", ResourceTable = "IncomeResource", Alias = "ir" },
                new { PermissionTable = "IOLInvestmentAssetPermissions", ResourceTable = "IOLInvestmentAssetResource", Alias = "iar" },
                new { PermissionTable = "IOLInvestmentPermissions", ResourceTable = "IOLInvestmentResource", Alias = "iir" },
                new { PermissionTable = "MovementPermissions", ResourceTable = "MovementResource", Alias = "mr" }
            };

            foreach (var migration in dataMigrations)
            {
                Up_MigrateResourceToPermissionData(migrationBuilder, migration.PermissionTable, migration.ResourceTable, migration.Alias);
            }
        }

        private void Up_MigrateResourceToPermissionData(MigrationBuilder migrationBuilder, string permissionTable, string resourceTable, string tableAlias)
        {
            migrationBuilder.Sql($@"
                INSERT INTO ""{permissionTable}"" (
                    ""Id"", 
                    ""ResourceId"", 
                    ""UserId"", 
                    ""PermissionLevels"", 
                    ""CreatedAt"", 
                    ""UpdatedAt"", 
                    ""Deactivated""
                )
                SELECT 
                    gen_random_uuid(),
                    {tableAlias}.""ResourceSourceId"",
                    ro.""UserId"",
                    ARRAY[3], -- Owner permission level
                    COALESCE(ro.""CreatedAt"", {tableAlias}.""CreatedAt""), -- Preserve original timestamp
                    COALESCE(ro.""UpdatedAt"", {tableAlias}.""UpdatedAt""),
                    COALESCE(ro.""Deactivated"", false)
                FROM ""{resourceTable}"" {tableAlias}
                INNER JOIN ""ResourceOwner"" ro ON {tableAlias}.""ResourceId"" = ro.""ResourceId""
                WHERE {tableAlias}.""Deactivated"" = false AND ro.""Deactivated"" = false;
            ");
        }

        private void Up_DropOldResourceTables(MigrationBuilder migrationBuilder)
        {
            var tablesToDrop = new[]
            {
                "CreditCardResource",
                "CurrencyExchangeRateResource",
                "DebitOriginResource",
                "DebitResource",
                "FundResource",
                "IncomeResource",
                "IOLInvestmentAssetResource",
                "IOLInvestmentResource",
                "MovementResource",
                "ResourceOwner",
                "Resource"
            };

            foreach (var tableName in tablesToDrop)
            {
                migrationBuilder.DropTable(name: tableName);
            }
        }

        // Helper methods for Down method
        private void Down_DropNewPermissionTables(MigrationBuilder migrationBuilder)
        {
            var permissionTables = new[]
            {
                "CreditCardPermissions",
                "CurrencyExchangeRatePermissions",
                "DebitOriginPermissions",
                "DebitPermissions",
                "FundPermissions",
                "IncomePermissions",
                "IOLInvestmentAssetPermissions",
                "IOLInvestmentPermissions",
                "MovementPermissions",
                "PermissionLevel"
            };

            foreach (var tableName in permissionTables)
            {
                migrationBuilder.DropTable(name: tableName);
            }
        }

        private void Down_CreateResourceTable(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Resource",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resource", x => x.Id);
                });
        }

        private void Down_CreateResourceMappingTable(MigrationBuilder migrationBuilder, string tableName, string sourceTable, string truncatedFkName = null)
        {
            var fkName = truncatedFkName ?? $"FK_{tableName}_{sourceTable}_ResourceSourceId";

            migrationBuilder.CreateTable(
                name: tableName,
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResourceSourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey($"PK_{tableName}", x => x.Id);
                    table.ForeignKey(
                        name: fkName,
                        column: x => x.ResourceSourceId,
                        principalTable: sourceTable,
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: $"FK_{tableName}_Resource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        private void Down_CreateResourceMappingTables(MigrationBuilder migrationBuilder)
        {
            Down_CreateResourceMappingTable(migrationBuilder, "CreditCardResource", "CreditCard");
            Down_CreateResourceMappingTable(migrationBuilder, "CurrencyExchangeRateResource", "CurrencyExchangeRate",
                "FK_CurrencyExchangeRateResource_CurrencyExchangeRate_ResourceS~");
            Down_CreateResourceMappingTable(migrationBuilder, "DebitOriginResource", "DebitOrigin");
            Down_CreateResourceMappingTable(migrationBuilder, "DebitResource", "Debit");
            Down_CreateResourceMappingTable(migrationBuilder, "FundResource", "Fund");
            Down_CreateResourceMappingTable(migrationBuilder, "IncomeResource", "Income");
            Down_CreateResourceMappingTable(migrationBuilder, "IOLInvestmentAssetResource", "IOLInvestmentAsset",
                "FK_IOLInvestmentAssetResource_IOLInvestmentAsset_ResourceSourc~");
            Down_CreateResourceMappingTable(migrationBuilder, "IOLInvestmentResource", "IOLInvestment");
            Down_CreateResourceMappingTable(migrationBuilder, "MovementResource", "Movement");
        }

        private void Down_CreateResourceOwnerTable(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResourceOwner",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false)
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
                        onDelete: ReferentialAction.Restrict);
                });
        }

        private void Down_CreateResourceTableIndexes(MigrationBuilder migrationBuilder)
        {
            // Indexes for resource mapping tables with unique constraints
            Down_CreateUniqueResourceIndex(migrationBuilder, "CreditCardResource");
            Down_CreateUniqueResourceIndex(migrationBuilder, "CurrencyExchangeRateResource");
            Down_CreateUniqueResourceIndex(migrationBuilder, "DebitResource");
            Down_CreateUniqueResourceIndex(migrationBuilder, "FundResource");

            // Indexes for resource mapping tables with single column indexes
            Down_CreateSingleResourceIndexes(migrationBuilder, "DebitOriginResource");
            Down_CreateSingleResourceIndexes(migrationBuilder, "IncomeResource");
            Down_CreateSingleResourceIndexes(migrationBuilder, "IOLInvestmentAssetResource");
            Down_CreateSingleResourceIndexes(migrationBuilder, "IOLInvestmentResource");
            Down_CreateSingleResourceIndexes(migrationBuilder, "MovementResource");

            // ResourceOwner table indexes
            migrationBuilder.CreateIndex(
                name: "IX_ResourceOwner_ResourceId",
                table: "ResourceOwner",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceOwner_UserId_ResourceId",
                table: "ResourceOwner",
                columns: ["UserId", "ResourceId"],
                unique: true);
        }

        private void Down_CreateUniqueResourceIndex(MigrationBuilder migrationBuilder, string tableName)
        {
            migrationBuilder.CreateIndex(
                name: $"IX_{tableName}_ResourceId_ResourceSourceId",
                table: tableName,
                columns: ["ResourceId", "ResourceSourceId"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: $"IX_{tableName}_ResourceSourceId",
                table: tableName,
                column: "ResourceSourceId");
        }

        private void Down_CreateSingleResourceIndexes(MigrationBuilder migrationBuilder, string tableName)
        {
            migrationBuilder.CreateIndex(
                name: $"IX_{tableName}_ResourceId",
                table: tableName,
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: $"IX_{tableName}_ResourceSourceId",
                table: tableName,
                column: "ResourceSourceId");
        }

        // Helper methods for migrating data back in Down method
        private void Down_MigrateDataFromPermissionTables(MigrationBuilder migrationBuilder)
        {
            var reverseMigrations = new[]
            {
                new { PermissionTable = "CreditCardPermissions", ResourceTable = "CreditCardResource" },
                new { PermissionTable = "CurrencyExchangeRatePermissions", ResourceTable = "CurrencyExchangeRateResource" },
                new { PermissionTable = "DebitOriginPermissions", ResourceTable = "DebitOriginResource" },
                new { PermissionTable = "DebitPermissions", ResourceTable = "DebitResource" },
                new { PermissionTable = "FundPermissions", ResourceTable = "FundResource" },
                new { PermissionTable = "IncomePermissions", ResourceTable = "IncomeResource" },
                new { PermissionTable = "IOLInvestmentAssetPermissions", ResourceTable = "IOLInvestmentAssetResource" },
                new { PermissionTable = "IOLInvestmentPermissions", ResourceTable = "IOLInvestmentResource" },
                new { PermissionTable = "MovementPermissions", ResourceTable = "MovementResource" }
            };

            foreach (var migration in reverseMigrations)
            {
                Down_MigratePermissionToResourceData(migrationBuilder, migration.PermissionTable, migration.ResourceTable);
            }
        }

        private void Down_MigratePermissionToResourceData(MigrationBuilder migrationBuilder, string permissionTable, string resourceTable)
        {
            migrationBuilder.Sql($@"
                WITH resource_data AS (
                    SELECT DISTINCT
                        p.""ResourceId"" as original_resource_id,
                        gen_random_uuid() as new_resource_id,
                        MIN(p.""CreatedAt"") as created_at,
                        BOOL_AND(NOT p.""Deactivated"") as deactivated,
                        MAX(COALESCE(p.""UpdatedAt"", p.""CreatedAt"")) as updated_at
                    FROM ""{permissionTable}"" p
                    WHERE p.""Deactivated"" = false
                    GROUP BY p.""ResourceId""
                ),
                inserted_resources AS (
                    INSERT INTO ""Resource"" (""Id"", ""CreatedAt"", ""Deactivated"", ""UpdatedAt"")
                    SELECT new_resource_id, created_at, deactivated, updated_at
                    FROM resource_data
                    RETURNING ""Id"", ""CreatedAt"", ""Deactivated"", ""UpdatedAt""
                ),
                inserted_mappings AS (
                    INSERT INTO ""{resourceTable}"" (
                        ""Id"", 
                        ""ResourceId"", 
                        ""ResourceSourceId"", 
                        ""CreatedAt"", 
                        ""Deactivated"", 
                        ""UpdatedAt""
                    )
                    SELECT 
                        gen_random_uuid(),
                        ir.""Id"",
                        rd.original_resource_id,
                        rd.created_at,
                        rd.deactivated,
                        rd.updated_at
                    FROM resource_data rd
                    INNER JOIN inserted_resources ir ON ir.""CreatedAt"" = rd.created_at 
                        AND ir.""Deactivated"" = rd.deactivated
                        AND COALESCE(ir.""UpdatedAt"", ir.""CreatedAt"") = rd.updated_at
                    RETURNING ""ResourceId"", ""ResourceSourceId""
                )
                INSERT INTO ""ResourceOwner"" (
                    ""Id"", 
                    ""ResourceId"", 
                    ""UserId"", 
                    ""CreatedAt"", 
                    ""Deactivated"", 
                    ""UpdatedAt""
                )
                SELECT DISTINCT
                    gen_random_uuid(),
                    im.""ResourceId"",
                    p.""UserId"",
                    p.""CreatedAt"",
                    p.""Deactivated"",
                    COALESCE(p.""UpdatedAt"", p.""CreatedAt"")
                FROM ""{permissionTable}"" p
                INNER JOIN inserted_mappings im ON im.""ResourceSourceId"" = p.""ResourceId""
                WHERE p.""Deactivated"" = false
                  AND 3 = ANY(p.""PermissionLevels"");
            ");
        }
    }
}
