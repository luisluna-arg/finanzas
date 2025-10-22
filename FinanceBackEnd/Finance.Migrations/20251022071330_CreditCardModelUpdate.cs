using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finance.Domain.Migrations
{
    /// <inheritdoc />
    public partial class CreditCardModelUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MinimumDue",
                table: "CreditCardStatement",
                type: "numeric(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UnappliedCredit",
                table: "CreditCard",
                type: "numeric(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "CreditCardPaymentPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditCardId = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    TotalInstallments = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCardPaymentPlan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditCardPaymentPlan_CreditCard_CreditCardId",
                        column: x => x.CreditCardId,
                        principalTable: "CreditCard",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreditCardPaymentPlanResource",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResourceSourceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCardPaymentPlanResource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditCardPaymentPlanResource_CreditCardPaymentPlan_ResourceSourceId",
                        column: x => x.ResourceSourceId,
                        principalTable: "CreditCardPaymentPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreditCardPaymentPlanResource_Resource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreditCardInstallment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentPlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PaymentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCardInstallment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditCardInstallment_CreditCardPaymentPlan_PaymentPlanId",
                        column: x => x.PaymentPlanId,
                        principalTable: "CreditCardPaymentPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreditCardInstallmentResource",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResourceSourceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCardInstallmentResource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditCardInstallmentResource_CreditCardInstallment_ResourceSourceId",
                        column: x => x.ResourceSourceId,
                        principalTable: "CreditCardInstallment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreditCardInstallmentResource_Resource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreditCardPayment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditCardId = table.Column<Guid>(type: "uuid", nullable: false),
                    StatementId = table.Column<Guid>(type: "uuid", nullable: true),
                    PaymentPlanId = table.Column<Guid>(type: "uuid", nullable: true),
                    InstallmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
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
                    table.ForeignKey(
                        name: "FK_CreditCardPayment_CreditCardInstallment_InstallmentId",
                        column: x => x.InstallmentId,
                        principalTable: "CreditCardInstallment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CreditCardPayment_CreditCardPaymentPlan_PaymentPlanId",
                        column: x => x.PaymentPlanId,
                        principalTable: "CreditCardPaymentPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CreditCardPaymentAllocation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    StatementId = table.Column<Guid>(type: "uuid", nullable: true),
                    PaymentPlanId = table.Column<Guid>(type: "uuid", nullable: true),
                    InstallmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    AllocationType = table.Column<string>(type: "text", nullable: true),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCardPaymentAllocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditCardPaymentAllocation_CreditCardPayment_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "CreditCardPayment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreditCardPaymentResource",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResourceSourceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCardPaymentResource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditCardPaymentResource_CreditCardPayment_ResourceSourceId",
                        column: x => x.ResourceSourceId,
                        principalTable: "CreditCardPayment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreditCardPaymentResource_Resource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreditCardPaymentAllocationResource",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Deactivated = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResourceSourceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCardPaymentAllocationResource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditCardPaymentAllocationResource_CreditCardPaymentAllocation_ResourceSourceId",
                        column: x => x.ResourceSourceId,
                        principalTable: "CreditCardPaymentAllocation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreditCardPaymentAllocationResource_Resource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardInstallment_PaymentId",
                table: "CreditCardInstallment",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardInstallment_PaymentPlanId",
                table: "CreditCardInstallment",
                column: "PaymentPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardInstallmentResource_ResourceId_ResourceSourceId",
                table: "CreditCardInstallmentResource",
                columns: ["ResourceId", "ResourceSourceId"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardInstallmentResource_ResourceSourceId",
                table: "CreditCardInstallmentResource",
                column: "ResourceSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardPayment_CreditCardId",
                table: "CreditCardPayment",
                column: "CreditCardId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardPayment_InstallmentId",
                table: "CreditCardPayment",
                column: "InstallmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardPayment_PaymentPlanId",
                table: "CreditCardPayment",
                column: "PaymentPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardPayment_StatementId",
                table: "CreditCardPayment",
                column: "StatementId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardPaymentAllocation_PaymentId",
                table: "CreditCardPaymentAllocation",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardPaymentAllocationResource_ResourceId_ResourceSourceId",
                table: "CreditCardPaymentAllocationResource",
                columns: ["ResourceId", "ResourceSourceId"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardPaymentAllocationResource_ResourceSourceId",
                table: "CreditCardPaymentAllocationResource",
                column: "ResourceSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardPaymentPlan_CreditCardId",
                table: "CreditCardPaymentPlan",
                column: "CreditCardId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardPaymentPlanResource_ResourceId_ResourceSourceId",
                table: "CreditCardPaymentPlanResource",
                columns: ["ResourceId", "ResourceSourceId"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardPaymentPlanResource_ResourceSourceId",
                table: "CreditCardPaymentPlanResource",
                column: "ResourceSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardPaymentResource_ResourceId_ResourceSourceId",
                table: "CreditCardPaymentResource",
                columns: ["ResourceId", "ResourceSourceId"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardPaymentResource_ResourceSourceId",
                table: "CreditCardPaymentResource",
                column: "ResourceSourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardInstallment_CreditCardPayment_PaymentId",
                table: "CreditCardInstallment",
                column: "PaymentId",
                principalTable: "CreditCardPayment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardInstallment_CreditCardPaymentPlan_PaymentPlanId",
                table: "CreditCardInstallment");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardPayment_CreditCardPaymentPlan_PaymentPlanId",
                table: "CreditCardPayment");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardInstallment_CreditCardPayment_PaymentId",
                table: "CreditCardInstallment");

            migrationBuilder.DropTable(
                name: "CreditCardInstallmentResource");

            migrationBuilder.DropTable(
                name: "CreditCardPaymentAllocationResource");

            migrationBuilder.DropTable(
                name: "CreditCardPaymentPlanResource");

            migrationBuilder.DropTable(
                name: "CreditCardPaymentResource");

            migrationBuilder.DropTable(
                name: "CreditCardPaymentAllocation");

            migrationBuilder.DropTable(
                name: "CreditCardPaymentPlan");

            migrationBuilder.DropTable(
                name: "CreditCardPayment");

            migrationBuilder.DropTable(
                name: "CreditCardInstallment");

            migrationBuilder.DropColumn(
                name: "MinimumDue",
                table: "CreditCardStatement");

            migrationBuilder.DropColumn(
                name: "UnappliedCredit",
                table: "CreditCard");
        }
    }
}
