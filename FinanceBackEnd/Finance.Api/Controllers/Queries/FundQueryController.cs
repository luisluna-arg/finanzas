using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Mapping;
using Finance.Application.Queries.Funds;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/funds")]
[Authorize(Policy = "AdminOrOwnerPolicy")]
public class FundQueryController(IMappingService mapper, IDispatcher dispatcher)
    : SecuredApiController
{
    protected IMappingService MappingService { get => mapper; }
    protected IDispatcher Dispatcher { get => dispatcher; }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetFundsQuery request)
    {
        var result = await Dispatcher.DispatchQueryAsync(request);
        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromQuery] GetSingleFundQuery request)
    {
        var result = await Dispatcher.DispatchQueryAsync(request);
        return Ok(result.Data);
    }

    [HttpGet("paginated")]
    public async Task<IActionResult> GetPaginated([FromQuery] GetPaginatedFundsQuery request)
    {
        var result = await Dispatcher.DispatchQueryAsync(request);
        return Ok(result.Data);
    }

    [HttpGet("latest/{appModuleId}")]
    public async Task<IActionResult> Latest(Guid appModuleId)
    {
        var result = await Dispatcher.DispatchQueryAsync(new GetLatestFundQuery(appModuleId));
        return Ok(result.Data);
    }
}