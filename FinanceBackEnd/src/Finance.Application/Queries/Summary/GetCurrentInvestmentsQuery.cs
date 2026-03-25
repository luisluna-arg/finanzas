using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Dtos.Summary;
using Finance.Application.Services;
using Finance.Domain.Models.Interfaces;
using Finance.Domain.SpecialTypes;
using Finance.Persistence;
using Finance.Persistence.Constants;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Summary;

public record GetCurrentInvestmentsQuery : IQuery<TotalInvestments>;

public class GetCurrentInvestmentsQueryHandler(FinanceDbContext db, CurrencyConversionService currencyConverter)
    : IQueryHandler<GetCurrentInvestmentsQuery, TotalInvestments>
{
    private readonly FinanceDbContext _db = db;
    private readonly CurrencyConversionService _currencyConverter = currencyConverter;

    public async Task<DataResult<TotalInvestments>> ExecuteAsync(GetCurrentInvestmentsQuery request, CancellationToken cancellationToken)
    {
        var result = new TotalInvestments();

        var maxTimeStamp = await _db.IOLInvestment.MaxAsync(o => (DateTime?)o.TimeStamp, cancellationToken);
        if (maxTimeStamp == null)
            return DataResult<TotalInvestments>.Success(result);

        var defaultCurrencyId = Guid.Parse(CurrencyConstants.DefaultCurrencyId);

        var investments = await _db.IOLInvestment
            .Include(o => o.Asset).ThenInclude(o => o.Type)
            .Include(o => o.Asset).ThenInclude(o => o.Currency)
            .Where(o => !o.Deactivated && o.TimeStamp == maxTimeStamp.Value)
            .OrderBy(o => o.Asset.Symbol)
            .ToArrayAsync(cancellationToken);

        var valuedHolders = investments
            .Select(o => (IAmountHolder)new InvestmentAmountHolder { CurrencyId = o.Asset.CurrencyId, Amount = o.Valued })
            .ToList();
        var convertedValues = (await _currencyConverter.ConvertCollection(valuedHolders, defaultCurrencyId)).ToArray();

        result.Items.AddRange(investments.Select((o, i) => new Investment()
        {
            Id = $"{o.Id}",
            Label = o.Asset.Symbol,
            Valued = o.Valued,
            AverageReturn = o.AverageReturn,
            ValuedDefaultCurrency = convertedValues[i]
        }));

        return DataResult<TotalInvestments>.Success(result);
    }

    private sealed class InvestmentAmountHolder : IAmountHolder
    {
        public Guid CurrencyId { get; set; }
        public Money Amount { get; set; }
    }
}
