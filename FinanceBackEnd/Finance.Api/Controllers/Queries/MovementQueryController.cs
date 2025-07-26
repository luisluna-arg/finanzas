using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Mapping;
using Finance.Application.Queries.Movements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/movements")]
[Authorize(Policy = "AdminOrOwnerPolicy")]
public class MovementQueryController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : SecuredApiController
{
    protected IMappingService MappingService { get => mapper; }
    protected IDispatcher<FinanceDispatchContext> Dispatcher { get => dispatcher; }
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetMovementsQuery request)
    {
        var result = await Dispatcher.DispatchQueryAsync(request);
        return Ok(result.Data);
    }

    [HttpGet("paginated")]
    public async Task<IActionResult> GetPaginated([FromQuery] GetPaginatedMovementsQuery request)
    {
        var result = await Dispatcher.DispatchQueryAsync(request);
        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromQuery] GetSingleMovementQuery request)
    {
        var result = await Dispatcher.DispatchQueryAsync(request);
        return Ok(result.Data);
    }

    [HttpGet("latest/{appModuleId}")]
    public async Task<IActionResult> Latest(Guid appModuleId)
    {
        var result = await Dispatcher.DispatchQueryAsync(new GetLatestMovementQuery(appModuleId));
        return Ok(result.Data);
    }
}
