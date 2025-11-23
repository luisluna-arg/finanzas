using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Finance.Domain.Migrations
{
    public partial class SimplifiedPermissionsModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            Up_CreatePermissionLevelTable(migrationBuilder);

            Up_CreatePermissionTable(migrationBuilder, "CreditCardPermissions", "CreditCard");
            Up_CreatePermissionTable(migrationBuilder, "CurrencyExchangeRatePermissions", "CurrencyExchangeRate", "FK_CurrencyExchangeRatePermissions_CurrencyExchangeRate_Resour~");
            Up_CreatePermissionTable(migrationBuilder, "DebitOriginPermissions", "DebitOrigin");
            Up_CreatePermissionTable(migrationBuilder, "DebitPermissions", "Debit");
            Up_CreatePermissionTable(migrationBuilder, "FundPermissions", "Fund");
            Up_CreatePermissionTable(migrationBuilder, "IncomePermissions", "Income");
            Up_CreatePermissionTable(migrationBuilder, "IOLInvestmentAssetPermissions", "IOLInvestmentAsset");
            Up_CreatePermissionTable(migrationBuilder, "IOLInvestmentPermissions", "IOLInvestment");
            Up_CreatePermissionTable(migrationBuilder, "MovementPermissions", "Movement");

            Up_MigrateDataToPermissionTables(migrationBuilder);

            RenameColumn(migrationBuilder, "CreditCardStatementId", "CreditCardStatementTransaction", "StatementId");
            RenameIndex(migrationBuilder, "IX_CreditCardStatementTransaction_CreditCardStatementId_Posted~", "CreditCardStatementTransaction", "IX_CreditCardStatementTransaction_StatementId_PostedDate");
            RenameColumn(migrationBuilder, "CreditCardStatementId", "CreditCardStatementAdjustment", "StatementId");
            RenameIndex(migrationBuilder, "IX_CreditCardStatementAdjustment_CreditCardStatementId_Created~", "CreditCardStatementAdjustment", "IX_CreditCardStatementAdjustment_StatementId_CreatedAt");

            Up_CreatePermissionIndexes(migrationBuilder, "CreditCardPermissions");
            Up_CreatePermissionIndexes(migrationBuilder, "CurrencyExchangeRatePermissions");
            Up_CreatePermissionIndexes(migrationBuilder, "DebitOriginPermissions");
            Up_CreatePermissionIndexes(migrationBuilder, "DebitPermissions");
            Up_CreatePermissionIndexes(migrationBuilder, "FundPermissions");
            Up_CreatePermissionIndexes(migrationBuilder, "IncomePermissions");
            Up_CreatePermissionIndexes(migrationBuilder, "IOLInvestmentAssetPermissions");
            Up_CreatePermissionIndexes(migrationBuilder, "IOLInvestmentPermissions");
            Up_CreatePermissionIndexes(migrationBuilder, "MovementPermissions");

            Up_AddStatementForeignKeys(migrationBuilder, "CreditCardStatementAdjustment");
            Up_AddStatementForeignKeys(migrationBuilder, "CreditCardStatementTransaction");

            DropForeignKey(migrationBuilder, "FK_CreditCard_Bank_BankId1", "CreditCard");
            DropForeignKey(migrationBuilder, "FK_CreditCardStatementAdjustment_CreditCardStatement_CreditCar~", "CreditCardStatementAdjustment");
            DropForeignKey(migrationBuilder, "FK_CreditCardStatementTransaction_CreditCardStatement_CreditCa~", "CreditCardStatementTransaction");

            DropTable(migrationBuilder, "CreditCardResource");
            DropTable(migrationBuilder, "CurrencyExchangeRateResource");
            DropTable(migrationBuilder, "DebitOriginResource");
            DropTable(migrationBuilder, "DebitResource");
            DropTable(migrationBuilder, "FundResource");
            DropTable(migrationBuilder, "IncomeResource");
            DropTable(migrationBuilder, "IOLInvestmentAssetResource");
            DropTable(migrationBuilder, "IOLInvestmentResource");
            DropTable(migrationBuilder, "MovementResource");
            DropTable(migrationBuilder, "ResourceOwner");
            DropTable(migrationBuilder, "Resource");

            DropIndex(migrationBuilder, "IX_CreditCard_BankId1", "CreditCard");
            DropColumn(migrationBuilder, "BankId1", "CreditCard");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            Down_CreateResourceOwnershipTables(migrationBuilder);
            Down_CreateEntityResourceTables(migrationBuilder);

            RenameColumn(migrationBuilder, "StatementId", "CreditCardStatementTransaction", "CreditCardStatementId");
            RenameIndex(migrationBuilder, "IX_CreditCardStatementTransaction_StatementId_PostedDate", "CreditCardStatementTransaction", "IX_CreditCardStatementTransaction_CreditCardStatementId_Posted~");
            RenameColumn(migrationBuilder, "StatementId", "CreditCardStatementAdjustment", "CreditCardStatementId");
            RenameIndex(migrationBuilder, "IX_CreditCardStatementAdjustment_StatementId_CreatedAt", "CreditCardStatementAdjustment", "IX_CreditCardStatementAdjustment_CreditCardStatementId_Created~");

            migrationBuilder.AddColumn<Guid>(
                name: "BankId1",
                table: "CreditCard",
                type: "uuid",
                nullable: true);

            CreateIndex(migrationBuilder, "IX_CreditCard_BankId1", "CreditCard", "BankId1");

            var fkAdjustmentName = GetStatementForeignKeyName("CreditCardStatementAdjustment");
            var fkTransactionName = GetStatementForeignKeyName("CreditCardStatementTransaction");
            DropForeignKey(migrationBuilder, fkAdjustmentName, "CreditCardStatementAdjustment");
            DropForeignKey(migrationBuilder, fkTransactionName, "CreditCardStatementTransaction");

            Down_MigratePermissionsToResources(migrationBuilder);
            Down_CreateResourceSourceIndexes(migrationBuilder);

            var fkBank = GetForeignKeyName("CreditCard", "Bank", "BankId1");
            AddForeignKey(migrationBuilder, fkBank, "CreditCard", "BankId1", "Bank", "Id");

            var fkAdjOriginal = GetForeignKeyName("CreditCardStatementAdjustment", "CreditCardStatement", "CreditCardStatementId");
            AddForeignKey(migrationBuilder, fkAdjOriginal, "CreditCardStatementAdjustment", "CreditCardStatementId", "CreditCardStatement", "Id", ReferentialAction.Cascade);

            var fkTxOriginal = GetForeignKeyName("CreditCardStatementTransaction", "CreditCardStatement", "CreditCardStatementId");
            AddForeignKey(migrationBuilder, fkTxOriginal, "CreditCardStatementTransaction", "CreditCardStatementId", "CreditCardStatement", "Id", ReferentialAction.Cascade);

            DropTables(migrationBuilder);
        }

        /// <summary>
        /// Add a foreign key from the given table's <c>StatementId</c> to <c>CreditCardStatement.Id</c>.
        /// Truncates the generated foreign-key name when it exceeds the database identifier length limit.
        /// </summary>
        private void Up_AddStatementForeignKeys(MigrationBuilder migrationBuilder, string tableName)
        {
            const int maxLength = 63;
            var foreignKeyName = $"FK_{tableName}_CreditCardStatement_Statement";
            foreignKeyName = foreignKeyName.Length > maxLength ? foreignKeyName.Substring(0, maxLength - 1) + "~" : foreignKeyName;
            AddForeignKey(migrationBuilder, foreignKeyName, tableName, "StatementId", "CreditCardStatement", "Id", ReferentialAction.Cascade);
        }

        /// <summary>
        /// Compute the foreign-key name used for statement relationships, truncating
        /// it to fit the database identifier length limit when needed.
        /// </summary>
        private string GetStatementForeignKeyName(string tableName)
        {
            const int maxLength = 63;
            var foreignKeyName = $"FK_{tableName}_CreditCardStatement_Statement";
            return foreignKeyName.Length > maxLength ? foreignKeyName.Substring(0, maxLength - 1) + "~" : foreignKeyName;
        }

        /// <summary>
        /// Compute a conventional foreign-key name for <c>tableName</c> -> <c>principalTableName</c>.<c>columnName</c>,
        /// truncating if it would exceed the identifier length limit.
        /// </summary>
        private string GetForeignKeyName(string tableName, string principalTableName, string columnName)
        {
            const int maxLength = 63;
            var foreignKeyName = $"FK_{tableName}_{principalTableName}_{columnName}";
            return foreignKeyName.Length > maxLength ? foreignKeyName.Substring(0, maxLength - 1) + "~" : foreignKeyName;
        }

        /// <summary>
        /// Create the standard indexes for a permission table: a unique composite index on
        /// <c>ResourceId, UserId</c> and a non-unique index on <c>UserId</c>.
        /// </summary>
        private void Up_CreatePermissionIndexes(MigrationBuilder migrationBuilder, string tableName)
        {
            Up_CreatePermissionIndex(migrationBuilder, tableName);
            CreateIndex(migrationBuilder, $"IX_{tableName}_UserId", tableName, "UserId");
        }

        /// <summary>
        /// Create the <c>PermissionLevel</c> lookup table and seed the default permission levels.
        /// </summary>
        private void Up_CreatePermissionLevelTable(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PermissionLevel",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionLevel", x => x.Id);
                });

            var utcNow = DateTime.UtcNow;
            migrationBuilder.InsertData(
                table: "PermissionLevel",
                columns: ["Id", "Name", "CreatedAt", "UpdatedAt", "Deactivated"],
                values: new object[,]
                {
                    { (short)PermissionLevelEnum.None, PermissionLevelEnum.None.ToString(), utcNow, utcNow, false },
                    { (short)PermissionLevelEnum.Read, PermissionLevelEnum.Read.ToString(), utcNow, utcNow, false },
                    { (short)PermissionLevelEnum.Write, PermissionLevelEnum.Write.ToString(), utcNow, utcNow, false },
                    { (short)PermissionLevelEnum.Owner, PermissionLevelEnum.Owner.ToString(), utcNow, utcNow, false }
                });
        }

        /// <summary>
        /// Create a permission table for the specified resource type. The created table contains
        /// resource and user references and stores permission level arrays.
        /// </summary>
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

        /// <summary>
        /// Iterate over known resource tables and migrate existing ownership data into
        /// the corresponding permission tables.
        /// </summary>
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

        /// <summary>
        /// Migrate ownership rows from an entity's resource table into the target permission table.
        /// Inserts owner-level permission entries for each distinct entity/user pair.
        /// </summary>
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
                    sub.""ResourceSourceId"",
                    sub.""UserId"",
                    ARRAY[3], -- Owner permission level
                    sub.""CreatedAt"",
                    sub.""UpdatedAt"",
                    sub.""Deactivated""
                FROM (
                    SELECT DISTINCT
                        {tableAlias}.""ResourceSourceId"",
                        ro.""UserId"",
                        COALESCE(ro.""CreatedAt"", {tableAlias}.""CreatedAt"") AS ""CreatedAt"",
                        COALESCE(ro.""UpdatedAt"", {tableAlias}.""UpdatedAt"") AS ""UpdatedAt"",
                        COALESCE(ro.""Deactivated"", false) AS ""Deactivated""
                    FROM ""{resourceTable}"" {tableAlias}
                    INNER JOIN ""ResourceOwner"" ro ON {tableAlias}.""ResourceId"" = ro.""ResourceId""
                    WHERE {tableAlias}.""Deactivated"" = false AND ro.""Deactivated"" = false
                ) sub
                WHERE NOT EXISTS (
                    SELECT 1 FROM ""{permissionTable}"" p
                    WHERE p.""ResourceId"" = sub.""ResourceSourceId"" AND p.""UserId"" = sub.""UserId""
                );
            ");
        }

        #region Down - Resource restore helpers

        /// <summary>
        /// Re-create the <c>Resource</c> and <c>ResourceOwner</c> tables needed by the previous ownership model.
        /// </summary>
        private void Down_CreateResourceOwnershipTables(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Resource",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resource", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResourceOwner",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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

        /// <summary>
        /// Re-create per-entity resource tables (e.g. <c>CreditCardResource</c>).
        /// </summary>
        private void Down_CreateEntityResourceTables(MigrationBuilder migrationBuilder)
        {
            var tables = new[]
            {
                "CreditCard",
                "CurrencyExchangeRate",
                "DebitOrigin",
                "Debit",
                "Fund",
                "Income",
                "IOLInvestmentAsset",
                "IOLInvestment",
                "Movement"
            };

            foreach (var table in tables)
            {
                Down_CreateEntityResourceTable(migrationBuilder, table);
            }
        }

        /// <summary>
        /// Re-create a single entity resource table for the given entity name.
        /// </summary>
        private void Down_CreateEntityResourceTable(MigrationBuilder migrationBuilder, string entityTableName)
        {
            var entityResourceTableName = $"{entityTableName}Resource";
            migrationBuilder.CreateTable(
                name: entityResourceTableName,
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResourceSourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey($"PK_{entityResourceTableName}", x => x.Id);
                    table.ForeignKey(
                        name: $"FK_{entityResourceTableName}_{entityTableName}_ResourceSourceId",
                        column: x => x.ResourceSourceId,
                        principalTable: entityTableName,
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: $"FK_{entityResourceTableName}_Resource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <summary>
        /// Create indexes used by entity resource tables to support lookups by resource and source id.
        /// </summary>
        private void Down_CreateResourceSourceIndexes(MigrationBuilder migrationBuilder)
        {
            var composedKeytables = new[]
            {
                "CreditCardResource",
                "CurrencyExchangeRateResource",
                "DebitResource",
                "FundResource"
            };

            foreach (var table in composedKeytables)
            {
                Down_CreateResourceSourceComposedIndexes(migrationBuilder, table);
            }

            var simpleKeytables = new[]
            {
                "DebitOriginResource",
                "IncomeResource",
                "IOLInvestmentAssetResource",
                "IOLInvestmentResource",
                "MovementResource"
            };

            foreach (var table in simpleKeytables)
            {
                Down_CreateResourceSourceSimpleIndexes(migrationBuilder, table);
            }

            CreateIndex(migrationBuilder, "IX_ResourceOwner_ResourceId", "ResourceOwner", "ResourceId");
            CreateIndex(migrationBuilder, "IX_ResourceOwner_UserId_ResourceId", "ResourceOwner", ["UserId", "ResourceId"], unique: true);
        }

        /// <summary>
        /// Create composed and source indexes for tables that have both <c>ResourceId</c> and <c>ResourceSourceId</c>.
        /// </summary>
        private void Down_CreateResourceSourceComposedIndexes(MigrationBuilder migrationBuilder, string tableName)
        {
            const int maxLength = 63;
            var indexName1 = $"IX_{tableName}_ResourceId_ResourceSourceId";
            indexName1 = indexName1.Length > maxLength ? indexName1.Substring(0, maxLength - 1) + "~" : indexName1;

            var indexName2 = $"IX_{tableName}_ResourceSourceId";
            indexName2 = indexName2.Length > maxLength ? indexName2.Substring(0, maxLength - 1) + "~" : indexName2;

            CreateIndex(migrationBuilder, indexName1, tableName, ["ResourceId", "ResourceSourceId"], unique: true);
            CreateIndex(migrationBuilder, indexName2, tableName, "ResourceSourceId");
        }

        /// <summary>
        /// Create simple indexes for tables that only require indexing on a single resource-related column.
        /// </summary>
        private void Down_CreateResourceSourceSimpleIndexes(MigrationBuilder migrationBuilder, string tableName)
        {
            const int maxLength = 63;
            var indexName1 = $"IX_{tableName}_ResourceId";
            indexName1 = indexName1.Length > maxLength ? indexName1.Substring(0, maxLength - 1) + "~" : indexName1;

            var indexName2 = $"IX_{tableName}_ResourceSourceId";
            indexName2 = indexName2.Length > maxLength ? indexName2.Substring(0, maxLength - 1) + "~" : indexName2;

            CreateIndex(migrationBuilder, indexName1, tableName, "ResourceId");
            CreateIndex(migrationBuilder, indexName2, tableName, "ResourceSourceId");
        }


        /// <summary>
        /// Drop the permission and lookup tables introduced by the simplified permissions model.
        /// </summary>
        private void DropTables(MigrationBuilder migrationBuilder)
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

            foreach (var table in permissionTables)
            {
                DropTable(migrationBuilder, table);
            }
        }

        /// <summary>
        /// Iterate over permission tables and migrate permission data back into the entity resource tables.
        /// </summary>
        private void Down_MigratePermissionsToResources(MigrationBuilder migrationBuilder)
        {
            var dataMigrations = new[]
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

            foreach (var migration in dataMigrations)
            {
                Down_MigratePermissionToResourceData(migrationBuilder, migration.PermissionTable, migration.ResourceTable);
            }
        }

        /// <summary>
        /// For a given permission table, rebuild <c>Resource</c>, the entity resource rows and <c>ResourceOwner</c>
        /// entries using a temporary mapping table to associate generated Resource ids with entity ids.
        /// </summary>
        private void Down_MigratePermissionToResourceData(MigrationBuilder migrationBuilder, string permissionTable, string resourceTable)
        {
            // For each distinct entity (permission.ResourceId) create a Resource, an EntityResource row
            // and ResourceOwner rows for every user that had a permission for that entity.
            // Uses a temporary mapping table to keep generated Resource.Id values.
            migrationBuilder.Sql($@"
                CREATE TEMP TABLE temp_{permissionTable}_mapping AS
                SELECT
                    p.""ResourceId"" AS ""ResourceSourceId"",
                    gen_random_uuid() AS ""ResourceId"",
                    MIN(p.""CreatedAt"") AS ""CreatedAt"",
                    MAX(p.""UpdatedAt"") AS ""UpdatedAt"",
                    bool_or(COALESCE(p.""Deactivated"", false)) AS ""Deactivated""
                FROM ""{permissionTable}"" p
                GROUP BY p.""ResourceId"";

                INSERT INTO ""Resource"" (""Id"", ""CreatedAt"", ""UpdatedAt"", ""Deactivated"")
                SELECT t.""ResourceId"", t.""CreatedAt"", t.""UpdatedAt"", t.""Deactivated""
                FROM temp_{permissionTable}_mapping t
                WHERE NOT EXISTS (SELECT 1 FROM ""Resource"" r WHERE r.""Id"" = t.""ResourceId"");

                -- Insert entity resource rows linking the generated Resource.Id with the entity (ResourceSourceId)
                INSERT INTO ""{resourceTable}"" (""Id"", ""ResourceId"", ""ResourceSourceId"", ""CreatedAt"", ""Deactivated"", ""UpdatedAt"")
                SELECT gen_random_uuid(), t.""ResourceId"", t.""ResourceSourceId"", t.""CreatedAt"", t.""Deactivated"", t.""UpdatedAt""
                FROM temp_{permissionTable}_mapping t
                WHERE NOT EXISTS (SELECT 1 FROM ""{resourceTable}"" er WHERE er.""ResourceSourceId"" = t.""ResourceSourceId"");

                -- Insert ResourceOwner rows for each permission entry (avoid duplicates)
                INSERT INTO ""ResourceOwner"" (""Id"", ""ResourceId"", ""UserId"", ""CreatedAt"", ""Deactivated"", ""UpdatedAt"")
                SELECT gen_random_uuid(), t.""ResourceId"", p.""UserId"", COALESCE(p.""CreatedAt"", t.""CreatedAt""), COALESCE(p.""Deactivated"", false), p.""UpdatedAt""
                FROM ""{permissionTable}"" p
                INNER JOIN temp_{permissionTable}_mapping t ON t.""ResourceSourceId"" = p.""ResourceId""
                WHERE NOT EXISTS (
                    SELECT 1 FROM ""ResourceOwner"" ro
                    WHERE ro.""ResourceId"" = t.""ResourceId"" AND ro.""UserId"" = p.""UserId""
                );

                DROP TABLE temp_{permissionTable}_mapping;
            ");
        }

        #endregion

        #region General - Utility helpers

        /// <summary>
        /// Create the unique composite index on <c>ResourceId, UserId</c> for a permission table.
        /// </summary>
        private void Up_CreatePermissionIndex(MigrationBuilder migrationBuilder, string tableName)
            => CreateIndex(migrationBuilder, $"IX_{tableName}_ResourceId_UserId", tableName, ["ResourceId", "UserId"], unique: true);

        /// <summary>
        /// Drop a constraint from the specified table if it exists (defensive drop to avoid runtime errors).
        /// </summary>
        private void DropForeignKey(MigrationBuilder migrationBuilder, string foreignKeyName, string tableName)
            => migrationBuilder.Sql($"ALTER TABLE \"{tableName}\" DROP CONSTRAINT IF EXISTS \"{foreignKeyName}\";");

        /// <summary>
        /// Drop a table if it exists (uses a defensive <c>IF EXISTS</c> drop).
        /// </summary>
        private void DropTable(MigrationBuilder migrationBuilder, string tableName)
            => migrationBuilder.Sql($@"DROP TABLE IF EXISTS ""{tableName}"";");

        /// <summary>
        /// Drop an index if it exists. Uses a defensive <c>DROP INDEX IF EXISTS</c> statement.
        /// </summary>
        private void DropIndex(MigrationBuilder migrationBuilder, string indexName, string tableName)
            => migrationBuilder.Sql($"DROP INDEX IF EXISTS \"{indexName}\";");

        /// <summary>
        /// Drop a column from a table if it exists (defensive operation to avoid errors during migration).
        /// </summary>
        private void DropColumn(MigrationBuilder migrationBuilder, string columnName, string tableName)
            => migrationBuilder.Sql($"ALTER TABLE \"{tableName}\" DROP COLUMN IF EXISTS \"{columnName}\";");

        /// <summary>
        /// Rename a column on a table.
        /// </summary>
        private void RenameColumn(MigrationBuilder migrationBuilder, string oldColumnName, string tableName, string newColumnName)
            => migrationBuilder.RenameColumn(table: tableName, name: oldColumnName, newName: newColumnName);

        /// <summary>
        /// Rename an index on a table.
        /// </summary>
        private void RenameIndex(MigrationBuilder migrationBuilder, string currentIndexName, string tableName, string newIndexName)
            => migrationBuilder.RenameIndex(table: tableName, name: currentIndexName, newName: newIndexName);

        /// <summary>
        /// Create an index on a single column.
        /// </summary>
        private void CreateIndex(MigrationBuilder migrationBuilder, string indexName, string tableName, string columnName, bool unique = false)
            => migrationBuilder.CreateIndex(name: indexName, table: tableName, column: columnName, unique: unique);

        /// <summary>
        /// Create an index on multiple columns.
        /// </summary>
        private void CreateIndex(MigrationBuilder migrationBuilder, string indexName, string tableName, string[] columns, bool unique = false)
            => migrationBuilder.CreateIndex(name: indexName, table: tableName, columns: columns, unique: unique);

        /// <summary>
        /// Add a foreign-key constraint between two tables.
        /// </summary>
        private void AddForeignKey(MigrationBuilder migrationBuilder, string name, string table, string column, string principalTable, string principalColumn)
            => migrationBuilder.AddForeignKey(name: name, table: table, column: column, principalTable: principalTable, principalColumn: principalColumn);

        /// <summary>
        /// Add a foreign-key constraint between two tables with an explicit delete behavior.
        /// </summary>
        private void AddForeignKey(MigrationBuilder migrationBuilder, string name, string table, string column, string principalTable, string principalColumn, ReferentialAction onDelete)
            => migrationBuilder.AddForeignKey(name: name, table: table, column: column, principalTable: principalTable, principalColumn: principalColumn, onDelete: onDelete);

        #endregion
    }
}
