using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Queries.Catalog;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/catalog")]
public class CatalogQueryController(IDispatcher<FinanceDispatchContext> dispatcher) : SecuredApiController
{
    [HttpGet("banks")]
    public async Task<IActionResult> GetBanks([FromQuery] GetCatalogBanksQuery query)
    {
        var result = await dispatcher.DispatchQueryAsync(query, HttpContext.Request);
        return Ok(result.Data);
    }

    [HttpGet("currencies")]
    public async Task<IActionResult> GetCurrencies([FromQuery] GetCatalogCurrenciesQuery query)
    {
        var result = await dispatcher.DispatchQueryAsync(query, HttpContext.Request);
        return Ok(result.Data);
    }

    [HttpGet("frequencies")]
    public async Task<IActionResult> GetFrequencies([FromQuery] GetCatalogFrequenciesQuery query)
    {
        var result = await dispatcher.DispatchQueryAsync(query, HttpContext.Request);
        return Ok(result.Data);
    }
}
