using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Dtos.BankCurrencies;
using Finance.Application.Mapping;
using Finance.Application.Queries.BankCurrencies;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/bank-currencies")]
public class BankCurrencyQueryController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : SecuredApiController
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetBankCurrenciesQuery request)
    {
        var result = await dispatcher.DispatchQueryAsync(request);
        return Ok(mapper.Map<BankCurrencyDto>(result.Data));
    }

    [HttpGet("{bankId}/{currencyId}")]
    public async Task<IActionResult> GetOne(Guid bankId, Guid currencyId)
    {
        var result = await dispatcher.DispatchQueryAsync(new GetBankCurrencyQuery(bankId, currencyId));
        if (result.Data == null) return NotFound();

        return Ok(mapper.Map<BankCurrencyDto>(result.Data));
    }
}
