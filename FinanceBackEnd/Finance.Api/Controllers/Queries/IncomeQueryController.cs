using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Mapping;
using Finance.Application.Queries.Incomes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/incomes")]
[Authorize(Policy = "AdminOrOwnerPolicy")]
public class IncomeQueryController(IMappingService mapper, IDispatcher dispatcher)
    : SecuredApiController
{
    protected IMappingService MappingService { get => mapper; }
    protected IDispatcher Dispatcher { get => dispatcher; }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetIncomesQuery request)
    {
        var result = await Dispatcher.DispatchQueryAsync(request);
        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromQuery] GetSingleIncomeQuery request)
    {
        var result = await Dispatcher.DispatchQueryAsync(request);
        return Ok(result.Data);
    }

    [HttpGet("paginated")]
    public async Task<IActionResult> GetPaginated([FromQuery] GetPaginatedIncomesQuery request)
    {
        var result = await Dispatcher.DispatchQueryAsync(request);
        return Ok(result.Data);
    }

    [HttpGet("latest/{appModuleId}")]
    public async Task<IActionResult> Latest(Guid appModuleId)
    {
        var result = await Dispatcher.DispatchQueryAsync(new GetLatestIncomeQuery(appModuleId));
        return Ok(result.Data);
    }
}