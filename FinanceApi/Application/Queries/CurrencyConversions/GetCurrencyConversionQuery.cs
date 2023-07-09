using FinanceApi.Domain.Models;

namespace FinanceApi.Application.Queries.CurrencyConversions;

public class GetCurrencyConversionQuery : GetSingleByIdQuery<CurrencyConversion, Guid>
{
}
