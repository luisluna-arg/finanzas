using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class MigrateCreditCardMovementsToPaymentPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Create PaymentPlans from unique combinations of (CreditCardId, PlanStart, PlanSize, Concept)
            // Only create plans where PlanSize > 1 (actual installment plans)
            migrationBuilder.Sql(@"
                INSERT INTO ""CreditCardPaymentPlan"" (""Id"", ""CreditCardId"", ""TotalAmount"", ""TotalInstallments"", ""CreatedAt"", ""Status"", ""Deactivated"")
                SELECT 
                    gen_random_uuid() as ""Id"",
                    ""CreditCardId"",
                    SUM(""Amount"") as ""TotalAmount"",
                    MAX(""PlanSize"") as ""TotalInstallments"",
                    MIN(""PlanStart"") as ""CreatedAt"",
                    0 as ""Status"",
                    false as ""Deactivated""
                FROM ""CreditCardMovement""
                WHERE ""PlanSize"" > 1
                GROUP BY ""CreditCardId"", ""PlanStart"", ""PlanSize"", ""Concept""
                ORDER BY ""CreditCardId"", MIN(""PlanStart"");
            ");

            // Step 2: Create Installments for each CreditCardMovement that belongs to a plan
            // Match movements to their corresponding PaymentPlan based on (CreditCardId, PlanStart, PlanSize, Concept)
            migrationBuilder.Sql(@"
                INSERT INTO ""CreditCardInstallment"" (""Id"", ""PaymentPlanId"", ""Number"", ""DueDate"", ""Amount"", ""PaidAmount"", ""Status"", ""PaymentId"", ""Deactivated"")
                SELECT 
                    gen_random_uuid() as ""Id"",
                    pp.""Id"" as ""PaymentPlanId"",
                    ccm.""PaymentNumber"" as ""Number"",
                    ccm.""TimeStamp"" as ""DueDate"",
                    ccm.""Amount"" as ""Amount"",
                    0 as ""PaidAmount"",
                    CASE 
                        WHEN ccm.""TimeStamp"" <= NOW() THEN 1  -- Paid if due date has passed (simplified assumption)
                        ELSE 0  -- Pending
                    END as ""Status"",
                    NULL as ""PaymentId"",
                    false as ""Deactivated""
                FROM ""CreditCardMovement"" ccm
                INNER JOIN ""CreditCardPaymentPlan"" pp 
                    ON ccm.""CreditCardId"" = pp.""CreditCardId""
                    AND ccm.""PlanStart"" = pp.""CreatedAt""
                    AND ccm.""PlanSize"" = pp.""TotalInstallments""
                WHERE ccm.""PlanSize"" > 1
                ORDER BY pp.""Id"", ccm.""PaymentNumber"";
            ");

            // Step 3: Update PaidAmount for installments where status is Paid (set to Amount)
            migrationBuilder.Sql(@"
                UPDATE ""CreditCardInstallment""
                SET ""PaidAmount"" = ""Amount""
                WHERE ""Status"" = 1;
            ");

            // Step 4: Create PaymentPlanResource records
            // Link each PaymentPlan to the same Resource as the first CreditCardMovement in that plan
            migrationBuilder.Sql(@"
                INSERT INTO ""CreditCardPaymentPlanResource"" (""Id"", ""ResourceId"", ""ResourceSourceId"", ""CreatedAt"", ""UpdatedAt"", ""Deactivated"")
                SELECT 
                    gen_random_uuid() as ""Id"",
                    ccmr.""ResourceId"",
                    pp.""Id"" as ""ResourceSourceId"",
                    NOW() as ""CreatedAt"",
                    NULL as ""UpdatedAt"",
                    false as ""Deactivated""
                FROM ""CreditCardPaymentPlan"" pp
                INNER JOIN LATERAL (
                    SELECT ccm.""Id"", ccm.""CreditCardId"", ccm.""PlanStart"", ccm.""PlanSize""
                    FROM ""CreditCardMovement"" ccm
                    WHERE ccm.""CreditCardId"" = pp.""CreditCardId""
                      AND ccm.""PlanStart"" = pp.""CreatedAt""
                      AND ccm.""PlanSize"" = pp.""TotalInstallments""
                    ORDER BY ccm.""PaymentNumber""
                    LIMIT 1
                ) ccm_first ON true
                INNER JOIN ""CreditCardMovementResource"" ccmr ON ccmr.""ResourceSourceId"" = ccm_first.""Id"";
            ");

            // Step 5: Create InstallmentResource records
            // Link each Installment to the same Resource as its corresponding CreditCardMovement
            migrationBuilder.Sql(@"
                INSERT INTO ""CreditCardInstallmentResource"" (""Id"", ""ResourceId"", ""ResourceSourceId"", ""CreatedAt"", ""UpdatedAt"", ""Deactivated"")
                SELECT 
                    gen_random_uuid() as ""Id"",
                    ccmr.""ResourceId"",
                    i.""Id"" as ""ResourceSourceId"",
                    NOW() as ""CreatedAt"",
                    NULL as ""UpdatedAt"",
                    false as ""Deactivated""
                FROM ""CreditCardInstallment"" i
                INNER JOIN ""CreditCardPaymentPlan"" pp ON i.""PaymentPlanId"" = pp.""Id""
                INNER JOIN ""CreditCardMovement"" ccm 
                    ON ccm.""CreditCardId"" = pp.""CreditCardId""
                    AND ccm.""PlanStart"" = pp.""CreatedAt""
                    AND ccm.""PlanSize"" = pp.""TotalInstallments""
                    AND ccm.""PaymentNumber"" = i.""Number""
                INNER JOIN ""CreditCardMovementResource"" ccmr ON ccmr.""ResourceSourceId"" = ccm.""Id"";
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Clean up migrated data in reverse order (respecting FK constraints)
            migrationBuilder.Sql(@"DELETE FROM ""CreditCardInstallmentResource"" WHERE ""ResourceSourceId"" IN (SELECT ""Id"" FROM ""CreditCardInstallment"");");
            migrationBuilder.Sql(@"DELETE FROM ""CreditCardPaymentPlanResource"" WHERE ""ResourceSourceId"" IN (SELECT ""Id"" FROM ""CreditCardPaymentPlan"");");
            migrationBuilder.Sql(@"DELETE FROM ""CreditCardInstallment"" WHERE ""PaymentPlanId"" IN (SELECT ""Id"" FROM ""CreditCardPaymentPlan"");");
            migrationBuilder.Sql(@"DELETE FROM ""CreditCardPaymentPlan"";");
        }
    }
}
