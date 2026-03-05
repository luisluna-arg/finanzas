using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class CreateCreditCardIssuers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create CreditCardIssuer records from distinct CreditCard names
            // Handle duplicates by trimming and comparing with uppercase
            migrationBuilder.Sql(@"
                INSERT INTO ""CreditCardIssuer"" (""Id"", ""Code"", ""Name"", ""Deactivated"")
                SELECT 
                    gen_random_uuid() as ""Id"",
                    UPPER(TRIM(""Name"")) as ""Code"",
                    TRIM(""Name"") as ""Name"",
                    false as ""Deactivated""
                FROM (
                    SELECT DISTINCT TRIM(""Name"") as ""Name""
                    FROM ""CreditCard""
                    WHERE ""Name"" IS NOT NULL 
                    AND TRIM(""Name"") != ''
                    AND ""Deactivated"" = false
                ) distinct_names;
            ");

            // Update CreditCard records to link them to the newly created CreditCardIssuer records
            migrationBuilder.Sql(@"
                UPDATE ""CreditCard""
                SET ""CreditCardIssuerId"" = issuer.""Id""
                FROM ""CreditCardIssuer"" issuer
                WHERE UPPER(TRIM(""CreditCard"".""Name"")) = UPPER(TRIM(issuer.""Name""))
                AND ""CreditCard"".""CreditCardIssuerId"" IS NULL
                AND ""CreditCard"".""Deactivated"" = false;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove the CreditCardIssuer references from CreditCard
            migrationBuilder.Sql(@"
                UPDATE ""CreditCard""
                SET ""CreditCardIssuerId"" = NULL
                WHERE ""CreditCardIssuerId"" IS NOT NULL;
            ");

            // Delete all CreditCardIssuer records that were created by this migration
            migrationBuilder.Sql(@"
                DELETE FROM ""CreditCardIssuer""
                WHERE ""Code"" IN (
                    SELECT DISTINCT UPPER(TRIM(""Name""))
                    FROM ""CreditCard""
                    WHERE ""Name"" IS NOT NULL 
                    AND TRIM(""Name"") != ''
                );
            ");
        }
    }
}
