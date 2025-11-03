using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class NewCreditCardModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Create temporary tables to backup existing data
            migrationBuilder.Sql(@"
                -- Backup existing CreditCardMovement data
                CREATE TEMP TABLE temp_credit_card_movements AS 
                SELECT * FROM ""CreditCardMovement"" WHERE ""Deactivated"" = false;
                
                -- Backup existing CreditCardMovementResource data
                CREATE TEMP TABLE temp_credit_card_movement_resources AS 
                SELECT * FROM ""CreditCardMovementResource"" WHERE ""Deactivated"" = false;
                
                -- Backup existing CreditCardStatementResource data
                CREATE TEMP TABLE temp_credit_card_statement_resources AS 
                SELECT * FROM ""CreditCardStatementResource"" WHERE ""Deactivated"" = false;
            ");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCard_Bank_BankId",
                table: "CreditCard");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardStatement_CreditCard_CreditCardId",
                table: "CreditCardStatement");

            migrationBuilder.DropTable(
                name: "CreditCardMovementResource");

            migrationBuilder.DropTable(
                name: "CreditCardStatementResource");

            migrationBuilder.DropTable(
                name: "CreditCardMovement");

            migrationBuilder.DropIndex(
                name: "IX_CreditCardStatement_CreditCardId",
                table: "CreditCardStatement");

            migrationBuilder.DropIndex(
                name: "IX_CreditCard_BankId",
                table: "CreditCard");

            migrationBuilder.RenameColumn(
                name: "CreditCardStatementId",
                table: "CreditCard",
                newName: "CurrentStatementId1");

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumDue",
                table: "CreditCardStatement",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CreditCard",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "BankId1",
                table: "CreditCard",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreditCardIssuerId",
                table: "CreditCard",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreditCardIssuerId1",
                table: "CreditCard",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CurrentStatementId",
                table: "CreditCard",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UnappliedCredit",
                table: "CreditCard",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Bank",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CreditCardIssuer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCardIssuer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CreditCardPayment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditCardId = table.Column<Guid>(type: "uuid", nullable: false),
                    StatementId = table.Column<Guid>(type: "uuid", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Method = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCardPayment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditCardPayment_CreditCardStatement_StatementId",
                        column: x => x.StatementId,
                        principalTable: "CreditCardStatement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CreditCardPayment_CreditCard_CreditCardId",
                        column: x => x.CreditCardId,
                        principalTable: "CreditCard",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CreditCardStatementAdjustment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditCardStatementId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCardStatementAdjustment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditCardStatementAdjustment_CreditCardStatement_CreditCar~",
                        column: x => x.CreditCardStatementId,
                        principalTable: "CreditCardStatement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreditCardStatementTransaction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditCardStatementId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditCardTransactionId = table.Column<Guid>(type: "uuid", nullable: true),
                    PostedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreditCardId = table.Column<Guid>(type: "uuid", nullable: true),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCardStatementTransaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditCardStatementTransaction_CreditCardStatement_CreditCa~",
                        column: x => x.CreditCardStatementId,
                        principalTable: "CreditCardStatement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreditCardStatementTransaction_CreditCard_CreditCardId",
                        column: x => x.CreditCardId,
                        principalTable: "CreditCard",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CreditCardTransaction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditCardId = table.Column<Guid>(type: "uuid", nullable: false),
                    StatementTransactionId = table.Column<Guid>(type: "uuid", nullable: true),
                    StatementTransactionId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TransactionType = table.Column<int>(type: "integer", maxLength: 50, nullable: false),
                    Concept = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Reference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreditCardId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCardTransaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditCardTransaction_CreditCardStatementTransaction_Statem~",
                        column: x => x.StatementTransactionId1,
                        principalTable: "CreditCardStatementTransaction",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CreditCardTransaction_CreditCard_CreditCardId",
                        column: x => x.CreditCardId,
                        principalTable: "CreditCard",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CreditCardTransaction_CreditCard_CreditCardId1",
                        column: x => x.CreditCardId1,
                        principalTable: "CreditCard",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardStatement_CreditCardId_ClosureDate",
                table: "CreditCardStatement",
                columns: ["CreditCardId", "ClosureDate"]);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCard_BankId_Name",
                table: "CreditCard",
                columns: ["BankId", "Name"]);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCard_BankId1",
                table: "CreditCard",
                column: "BankId1");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCard_CreditCardIssuerId",
                table: "CreditCard",
                column: "CreditCardIssuerId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCard_CreditCardIssuerId1",
                table: "CreditCard",
                column: "CreditCardIssuerId1");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCard_CurrentStatementId1",
                table: "CreditCard",
                column: "CurrentStatementId1");

            migrationBuilder.CreateIndex(
                name: "IX_Bank_Name",
                table: "Bank",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardIssuer_Code",
                table: "CreditCardIssuer",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardPayment_CreditCardId_Timestamp",
                table: "CreditCardPayment",
                columns: ["CreditCardId", "Timestamp"]);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardPayment_StatementId",
                table: "CreditCardPayment",
                column: "StatementId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardStatementAdjustment_CreditCardStatementId_Created~",
                table: "CreditCardStatementAdjustment",
                columns: ["CreditCardStatementId", "CreatedAt"]);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardStatementTransaction_CreditCardId",
                table: "CreditCardStatementTransaction",
                column: "CreditCardId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardStatementTransaction_CreditCardStatementId_Posted~",
                table: "CreditCardStatementTransaction",
                columns: ["CreditCardStatementId", "PostedDate"]);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardStatementTransaction_CreditCardTransactionId",
                table: "CreditCardStatementTransaction",
                column: "CreditCardTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardTransaction_CreditCardId_Timestamp",
                table: "CreditCardTransaction",
                columns: ["CreditCardId", "Timestamp"]);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardTransaction_CreditCardId1",
                table: "CreditCardTransaction",
                column: "CreditCardId1");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardTransaction_StatementTransactionId1",
                table: "CreditCardTransaction",
                column: "StatementTransactionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCard_Bank_BankId",
                table: "CreditCard",
                column: "BankId",
                principalTable: "Bank",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCard_Bank_BankId1",
                table: "CreditCard",
                column: "BankId1",
                principalTable: "Bank",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCard_CreditCardIssuer_CreditCardIssuerId",
                table: "CreditCard",
                column: "CreditCardIssuerId",
                principalTable: "CreditCardIssuer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCard_CreditCardIssuer_CreditCardIssuerId1",
                table: "CreditCard",
                column: "CreditCardIssuerId1",
                principalTable: "CreditCardIssuer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCard_CreditCardStatement_CurrentStatementId1",
                table: "CreditCard",
                column: "CurrentStatementId1",
                principalTable: "CreditCardStatement",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardStatement_CreditCard_CreditCardId",
                table: "CreditCardStatement",
                column: "CreditCardId",
                principalTable: "CreditCard",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardStatementTransaction_CreditCardTransaction_Credit~",
                table: "CreditCardStatementTransaction",
                column: "CreditCardTransactionId",
                principalTable: "CreditCardTransaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            // Step 2: Migrate existing data to new structure
            migrationBuilder.Sql(@"
                -- Migrate CreditCardMovement data to CreditCardTransaction
                INSERT INTO ""CreditCardTransaction"" (
                    ""Id"", 
                    ""CreditCardId"", 
                    ""Timestamp"", 
                    ""TransactionType"", 
                    ""Concept"", 
                    ""Amount"", 
                    ""Reference"",
                    ""Deactivated""
                )
                SELECT 
                    ""Id"",
                    ""CreditCardId"",
                    ""TimeStamp"",
                    1 as ""TransactionType"", -- Assuming 1 represents a purchase/charge
                    ""Concept"",
                    ""Amount"",
                    CASE 
                        WHEN ""PaymentNumber"" > 0 AND ""PlanSize"" > 0 
                        THEN CONCAT('Installment ', ""PaymentNumber"", ' of ', ""PlanSize"", ' - Started: ', ""PlanStart""::date)
                        ELSE NULL
                    END as ""Reference"",
                    ""Deactivated""
                FROM temp_credit_card_movements;

                -- Create corresponding CreditCardStatementTransaction entries
                -- This assumes each transaction should appear on a statement
                INSERT INTO ""CreditCardStatementTransaction"" (
                    ""Id"",
                    ""CreditCardStatementId"",
                    ""CreditCardTransactionId"",
                    ""PostedDate"",
                    ""Amount"",
                    ""Description"",
                    ""Deactivated""
                )
                SELECT 
                    gen_random_uuid() as ""Id"",
                    cs.""Id"" as ""CreditCardStatementId"",
                    tcm.""Id"" as ""CreditCardTransactionId"",
                    tcm.""TimeStamp"" as ""PostedDate"",
                    tcm.""Amount"",
                    tcm.""Concept"" as ""Description"",
                    false as ""Deactivated""
                FROM temp_credit_card_movements tcm
                INNER JOIN ""CreditCardStatement"" cs ON cs.""CreditCardId"" = tcm.""CreditCardId""
                WHERE tcm.""TimeStamp"" >= cs.""ClosureDate"" - INTERVAL '30 days'
                  AND tcm.""TimeStamp"" <= cs.""ClosureDate"";

                -- Create default statements for credit cards that don't have any
                INSERT INTO ""CreditCardStatement"" (
                    ""Id"",
                    ""CreditCardId"",
                    ""ClosureDate"",
                    ""ExpiringDate"",
                    ""MinimumDue"",
                    ""Deactivated""
                )
                SELECT 
                    gen_random_uuid() as ""Id"",
                    tcm.""CreditCardId"",
                    DATE_TRUNC('month', MAX(tcm.""TimeStamp"")) + INTERVAL '1 month' - INTERVAL '1 day' as ""ClosureDate"",
                    DATE_TRUNC('month', MAX(tcm.""TimeStamp"")) + INTERVAL '2 months' - INTERVAL '1 day' as ""ExpiringDate"",
                    0 as ""MinimumDue"",
                    false as ""Deactivated""
                FROM temp_credit_card_movements tcm
                WHERE NOT EXISTS (
                    SELECT 1 FROM ""CreditCardStatement"" cs 
                    WHERE cs.""CreditCardId"" = tcm.""CreditCardId""
                )
                GROUP BY tcm.""CreditCardId"";

                -- For movements without a matching statement, create them with the latest statement
                INSERT INTO ""CreditCardStatementTransaction"" (
                    ""Id"",
                    ""CreditCardStatementId"",
                    ""CreditCardTransactionId"",
                    ""PostedDate"",
                    ""Amount"",
                    ""Description"",
                    ""Deactivated""
                )
                SELECT 
                    gen_random_uuid() as ""Id"",
                    (SELECT ""Id"" FROM ""CreditCardStatement"" WHERE ""CreditCardId"" = tcm.""CreditCardId"" ORDER BY ""ClosureDate"" DESC LIMIT 1) as ""CreditCardStatementId"",
                    tcm.""Id"" as ""CreditCardTransactionId"",
                    tcm.""TimeStamp"" as ""PostedDate"",
                    tcm.""Amount"",
                    tcm.""Concept"" as ""Description"",
                    false as ""Deactivated""
                FROM temp_credit_card_movements tcm
                WHERE NOT EXISTS (
                    SELECT 1 FROM ""CreditCardStatementTransaction"" cst 
                    WHERE cst.""CreditCardTransactionId"" = tcm.""Id""
                );
            ");

            // Step 3: Data validation and cleanup
            migrationBuilder.Sql(@"
                -- Validate migration: Check if all movements were migrated
                DO $$ 
                DECLARE
                    original_count INTEGER;
                    migrated_count INTEGER;
                BEGIN
                    SELECT COUNT(*) INTO original_count FROM temp_credit_card_movements;
                    SELECT COUNT(*) INTO migrated_count FROM ""CreditCardTransaction"" 
                    WHERE ""Id"" IN (SELECT ""Id"" FROM temp_credit_card_movements);
                    
                    IF original_count != migrated_count THEN
                        RAISE EXCEPTION 'Data migration failed: Original movements: %, Migrated: %', original_count, migrated_count;
                    END IF;
                    
                    RAISE NOTICE 'Successfully migrated % credit card movements to new structure', original_count;
                END $$;
                
                -- Clean up temporary tables
                DROP TABLE IF EXISTS temp_credit_card_movements;
                DROP TABLE IF EXISTS temp_credit_card_movement_resources;
                DROP TABLE IF EXISTS temp_credit_card_statement_resources;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditCard_Bank_BankId",
                table: "CreditCard");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCard_Bank_BankId1",
                table: "CreditCard");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCard_CreditCardIssuer_CreditCardIssuerId",
                table: "CreditCard");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCard_CreditCardIssuer_CreditCardIssuerId1",
                table: "CreditCard");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCard_CreditCardStatement_CurrentStatementId1",
                table: "CreditCard");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardStatement_CreditCard_CreditCardId",
                table: "CreditCardStatement");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardStatementTransaction_CreditCardTransaction_Credit~",
                table: "CreditCardStatementTransaction");

            migrationBuilder.DropTable(
                name: "CreditCardIssuer");

            migrationBuilder.DropTable(
                name: "CreditCardPayment");

            migrationBuilder.DropTable(
                name: "CreditCardStatementAdjustment");

            migrationBuilder.DropTable(
                name: "CreditCardTransaction");

            migrationBuilder.DropTable(
                name: "CreditCardStatementTransaction");

            migrationBuilder.DropIndex(
                name: "IX_CreditCardStatement_CreditCardId_ClosureDate",
                table: "CreditCardStatement");

            migrationBuilder.DropIndex(
                name: "IX_CreditCard_BankId_Name",
                table: "CreditCard");

            migrationBuilder.DropIndex(
                name: "IX_CreditCard_BankId1",
                table: "CreditCard");

            migrationBuilder.DropIndex(
                name: "IX_CreditCard_CreditCardIssuerId",
                table: "CreditCard");

            migrationBuilder.DropIndex(
                name: "IX_CreditCard_CreditCardIssuerId1",
                table: "CreditCard");

            migrationBuilder.DropIndex(
                name: "IX_CreditCard_CurrentStatementId1",
                table: "CreditCard");

            migrationBuilder.DropIndex(
                name: "IX_Bank_Name",
                table: "Bank");

            migrationBuilder.DropColumn(
                name: "MinimumDue",
                table: "CreditCardStatement");

            migrationBuilder.DropColumn(
                name: "BankId1",
                table: "CreditCard");

            migrationBuilder.DropColumn(
                name: "CreditCardIssuerId",
                table: "CreditCard");

            migrationBuilder.DropColumn(
                name: "CreditCardIssuerId1",
                table: "CreditCard");

            migrationBuilder.DropColumn(
                name: "CurrentStatementId",
                table: "CreditCard");

            migrationBuilder.DropColumn(
                name: "UnappliedCredit",
                table: "CreditCard");

            migrationBuilder.RenameColumn(
                name: "CurrentStatementId1",
                table: "CreditCard",
                newName: "CreditCardStatementId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CreditCard",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Bank",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.CreateTable(
                name: "CreditCardMovement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditCardId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    AmountDollars = table.Column<decimal>(type: "numeric", nullable: false),
                    Concept = table.Column<string>(type: "text", nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false),
                    PaymentNumber = table.Column<int>(type: "integer", nullable: false),
                    PlanSize = table.Column<int>(type: "integer", nullable: false),
                    PlanStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCardMovement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditCardMovement_CreditCard_CreditCardId",
                        column: x => x.CreditCardId,
                        principalTable: "CreditCard",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreditCardStatementResource",
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
                name: "CreditCardMovementResource",
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

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardStatement_CreditCardId",
                table: "CreditCardStatement",
                column: "CreditCardId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCard_BankId",
                table: "CreditCard",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardMovement_CreditCardId",
                table: "CreditCardMovement",
                column: "CreditCardId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardMovementResource_ResourceId_ResourceSourceId",
                table: "CreditCardMovementResource",
                columns: ["ResourceId", "ResourceSourceId"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardMovementResource_ResourceSourceId",
                table: "CreditCardMovementResource",
                column: "ResourceSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardStatementResource_ResourceId_ResourceSourceId",
                table: "CreditCardStatementResource",
                columns: ["ResourceId", "ResourceSourceId"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardStatementResource_ResourceSourceId",
                table: "CreditCardStatementResource",
                column: "ResourceSourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCard_Bank_BankId",
                table: "CreditCard",
                column: "BankId",
                principalTable: "Bank",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardStatement_CreditCard_CreditCardId",
                table: "CreditCardStatement",
                column: "CreditCardId",
                principalTable: "CreditCard",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
