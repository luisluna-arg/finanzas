using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Dtos.CreditCards;
using Finance.Application.Mapping;
using Finance.Application.Queries.CreditCards;
using Finance.Application.Services;
using Finance.Domain.Models.CreditCards;
using Finance.Domain.Models.Interfaces;
using Finance.Persistence.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/credit-card-statement-transactions")]
[Authorize(Policy = "AdminOrOwnerPolicy")]
public class CreditCardStatementTransactionQueryController(
    IMappingService mapper,
    IDispatcher<FinanceDispatchContext> dispatcher,
    CurrencyConversionService currencyConversionService)
    : ApiBaseQueryController<CreditCardTransaction, Guid, CreditCardTransactionDto>(mapper, dispatcher)
{
    [HttpGet("latest")]
    public async Task<IActionResult> GetLatest([FromQuery] GetLatestCreditCardTransactionsFromStatementsQuery query)
    {
        var dataResult = await Dispatcher.DispatchQueryAsync(query, Request);
        var dtos = dataResult.Data.Select(MappingService.Map<CreditCardTransactionDto>).ToList();

        var defaultCurrencyId = Guid.Parse(CurrencyConstants.DefaultCurrencyId);
        var convertedAmounts = await currencyConversionService.ConvertCollection(
            dtos.Cast<IAmountHolder>().ToList(),
            defaultCurrencyId);

        var convertedList = convertedAmounts.ToList();
        for (int i = 0; i < dtos.Count; i++)
            dtos[i].ConvertedAmount = convertedList[i];

        return Ok(dtos);
    }
}
