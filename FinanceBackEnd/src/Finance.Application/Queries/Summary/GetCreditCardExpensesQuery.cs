using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Dtos.Summary;
using Finance.Application.Queries.CreditCards;
using Finance.Application.Services;
using Finance.Domain.Models.Interfaces;
using Finance.Domain.SpecialTypes;
using Finance.Persistence.Constants;

namespace Finance.Application.Queries.Summary;

public record GetCreditCardExpensesQuery : IQuery<Expense>;

public class GetCreditCardExpensesQueryHandler(
    IDispatcher<FinanceDispatchContext> dispatcher,
    CurrencyConversionService currencyConverter)
    : IQueryHandler<GetCreditCardExpensesQuery, Expense>
{
    private readonly IDispatcher<FinanceDispatchContext> _dispatcher = dispatcher;
    private readonly CurrencyConversionService _currencyConverter = currencyConverter;

    public async Task<DataResult<Expense>> ExecuteAsync(GetCreditCardExpensesQuery request, CancellationToken cancellationToken)
    {
        var defaultCurrencyId = Guid.Parse(CurrencyConstants.DefaultCurrencyId);

        var transactionsResult = await _dispatcher.DispatchQueryAsync(
            new GetLatestCreditCardTransactionsFromStatementsQuery
            {
                IncludeDeactivated = false,
                IncludeExpiredStatements = true,
            });

        var holders = transactionsResult.Data
            .Select(t => (IAmountHolder)new TransactionAmountHolder { CurrencyId = t.CurrencyId, Amount = t.Amount })
            .ToList();

        var convertedAmounts = await _currencyConverter.ConvertCollection(holders, defaultCurrencyId);
        var total = convertedAmounts.Sum(m => (decimal)m);

        return DataResult<Expense>.Success(new Expense()
        {
            Id = "creditCards",
            Label = "Tarjetas de crédito",
            Value = total
        });
    }

    private sealed class TransactionAmountHolder : IAmountHolder
    {
        public Guid CurrencyId { get; set; }
        public Money Amount { get; set; }
    }
}
