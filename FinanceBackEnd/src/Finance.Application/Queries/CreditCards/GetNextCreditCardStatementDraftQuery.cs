using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Base.Handlers;
using Finance.Application.Dtos.CreditCards;
using Finance.Application.Mapping;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.CreditCards;

public record GetNextCreditCardStatementDraftQuery : IQuery<CreditCardStatementDraftDto>
{
    public Guid CreditCardId { get; set; }
}

public class GetNextCreditCardStatementDraftQueryHandler(FinanceDbContext db, IMappingService mapper)
    : BaseQueryHandler<GetNextCreditCardStatementDraftQuery, CreditCardStatementDraftDto>(db)
{
    public override async Task<DataResult<CreditCardStatementDraftDto>> ExecuteAsync(
        GetNextCreditCardStatementDraftQuery request, CancellationToken cancellationToken)
    {
        var currentStatement = await DbContext.CreditCardStatement
            .Where(s => s.CreditCardId == request.CreditCardId && !s.Deactivated)
            .OrderByDescending(s => s.ClosureDate)
            .FirstOrDefaultAsync(cancellationToken);

        if (currentStatement == null)
            return DataResult<CreditCardStatementDraftDto>.Failure("No existing statement found for this credit card.");

        var transactionIds = await DbContext.CreditCardStatementTransaction
            .Where(st => st.StatementId == currentStatement.Id && st.CreditCardTransactionId != null)
            .Select(st => st.CreditCardTransactionId!.Value)
            .ToListAsync(cancellationToken);

        var transactions = await DbContext.CreditCardTransaction
            .Include(t => t.Currency)
            .Include(t => t.PaymentPlan)
            .Where(t => transactionIds.Contains(t.Id) && !t.Deactivated)
            .ToListAsync(cancellationToken);

        var continuing = transactions
            .Where(t => t.PaymentPlan != null && t.InstallmentNumber < t.PaymentPlan.TotalInstallments)
            .Select(t => new CreditCardStatementDraftRowDto
            {
                PaymentPlanId = t.PaymentPlan!.Id,
                BaseConcept = t.PaymentPlan.BaseConcept,
                NextInstallmentNumber = t.InstallmentNumber + 1,
                TotalInstallments = t.PaymentPlan.TotalInstallments,
                SuggestedAmount = t.Amount,
                CurrencyId = t.CurrencyId,
                SourceTransactionId = t.Id,
            })
            .ToList();

        var eligible = transactions
            .Where(t => t.PaymentPlan == null || t.InstallmentNumber == t.PaymentPlan.TotalInstallments)
            .ToList();

        var draft = new CreditCardStatementDraftDto
        {
            CreditCardId = request.CreditCardId,
            CurrentStatementId = currentStatement.Id,
            SuggestedClosureDate = currentStatement.ClosureDate.AddMonths(1),
            SuggestedExpiringDate = currentStatement.ExpiringDate.AddMonths(1),
            ContinuingInstallments = continuing,
            EligibleNonPlanTransactions = mapper.Map<CreditCardTransactionDto>(eligible),
        };

        return DataResult<CreditCardStatementDraftDto>.Success(draft);
    }
}
