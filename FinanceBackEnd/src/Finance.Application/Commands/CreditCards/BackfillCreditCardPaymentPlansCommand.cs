using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commands.Base;
using Finance.Application.Services.CreditCards;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Commands.CreditCards;

/// <summary>
/// Links historical CreditCardTransaction rows into CreditCardPaymentPlan records using the same
/// resolver that live imports and manual entry use, so backfilled data follows identical rules.
/// Idempotent (only targets rows with PaymentPlanId == null) and safe to re-run after a partial failure.
/// </summary>
public class BackfillCreditCardPaymentPlansCommandHandler : BaseResponselessHandler<BackfillCreditCardPaymentPlansCommand>
{
    private const int BatchSize = 500;

    private readonly IsAdminUser _isAdminUser;
    private readonly CreditCardPaymentPlanResolver _paymentPlanResolver;

    public BackfillCreditCardPaymentPlansCommandHandler(
        FinanceDbContext db, IsAdminUser isAdminUser, CreditCardPaymentPlanResolver paymentPlanResolver)
        : base(db)
    {
        _isAdminUser = isAdminUser;
        _paymentPlanResolver = paymentPlanResolver;
    }

    public override async Task<CommandResult> ExecuteAsync(
        BackfillCreditCardPaymentPlansCommand command, CancellationToken cancellationToken)
    {
        var (isAdmin, _) = await _isAdminUser.IsSatisfiedAsync(cancellationToken);
        if (!isAdmin) return CommandResult.Failure("Admin only");

        var pendingIds = await DbContext.CreditCardTransaction
            .IgnoreQueryFilters()
            .Where(t => t.PaymentPlanId == null)
            .OrderBy(t => t.CreditCardId)
            .ThenBy(t => t.Timestamp)
            .ThenBy(t => t.Id)
            .Select(t => t.Id)
            .ToListAsync(cancellationToken);

        foreach (var chunk in pendingIds.Chunk(BatchSize))
        {
            var transactions = await DbContext.CreditCardTransaction
                .IgnoreQueryFilters()
                .Where(t => chunk.Contains(t.Id))
                .OrderBy(t => t.CreditCardId)
                .ThenBy(t => t.Timestamp)
                .ThenBy(t => t.Id)
                .ToListAsync(cancellationToken);

            foreach (var tx in transactions)
            {
                var (paymentPlanId, installmentNumber) = await _paymentPlanResolver.ResolvePlanAsync(
                    tx.CreditCardId, tx.CurrencyId, tx.Concept, tx.TransactionType, null, cancellationToken, tx.Id);

                tx.PaymentPlanId = paymentPlanId;
                tx.InstallmentNumber = installmentNumber;
            }

            await DbContext.SaveChangesAsync(cancellationToken);
        }

        return CommandResult.Success();
    }
}

public class BackfillCreditCardPaymentPlansCommand : ICommand;
