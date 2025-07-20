using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Mapping;
using Finance.Application.Queries.Debits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/debits/{frequency}")]
[Authorize(Policy = "AdminOrOwnerPolicy")]
public class DebitQueryController(IMappingService mapper, IDispatcher dispatcher)
    : SecuredApiController
{
    protected IMappingService MappingService { get => mapper; }
    protected IDispatcher Dispatcher { get => dispatcher; }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetAllDebitsQuery request)
    {
        var result = await Dispatcher.DispatchQueryAsync(request);
        return Ok(result.Data);
    }

    [HttpGet]
    [Route("latest")]
    public async Task<IActionResult> Latest([FromQuery] GetLatestDebitsQuery request)
    {
        var result = await Dispatcher.DispatchQueryAsync(request);
        return Ok(result.Data);
    }

    [HttpGet("paginated")]
    public async Task<IActionResult> GetPaginated([FromQuery] GetPaginatedDebitsQuery request)
    {
        var result = await Dispatcher.DispatchQueryAsync(request);
        return Ok(result.Data);
    }
}
