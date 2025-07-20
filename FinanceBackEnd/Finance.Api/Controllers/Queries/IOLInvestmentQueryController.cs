using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Mapping;
using Finance.Application.Queries.IOLInvestments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/iol-investment")]
[Authorize(Policy = "AdminOrOwnerPolicy")]
public class IOLInvestmentQueryController(IMappingService mapper, IDispatcher dispatcher)
    : SecuredApiController
{
    protected IMappingService MappingService { get => mapper; }
    protected IDispatcher Dispatcher { get => dispatcher; }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetIOLInvestmentsQuery request)
    {
        var result = await Dispatcher.DispatchQueryAsync(request);
        return Ok(result.Data);
    }

    [HttpGet("paginated")]
    public async Task<IActionResult> GetPaginated([FromQuery] GetPaginatedIOLInvestmentsQuery request)
    {
        var result = await Dispatcher.DispatchQueryAsync(request);
        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromQuery] GetSingleIOLInvestmentQuery request)
    {
        var result = await Dispatcher.DispatchQueryAsync(request);
        return Ok(result.Data);
    }
}
