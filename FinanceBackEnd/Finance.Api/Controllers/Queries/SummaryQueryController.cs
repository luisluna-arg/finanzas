using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Mapping;
using Finance.Application.Queries.Summary;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

// TODO I should be able to set Owner policy here, check authorization policies
[Route("api/summary")]
public class SummaryQueryController(IMappingService mapper, IDispatcher dispatcher)
    : SecuredApiController
{
    protected IMappingService MappingService { get => mapper; }
    protected IDispatcher Dispatcher { get => dispatcher; }

    [HttpGet("currentFunds")]
    public async Task<IActionResult> TotalFunds([FromQuery] GetCurrentFundsQuery request)
    {
        var result = await Dispatcher.DispatchQueryAsync(request);
        return Ok(result.Data);
    }

    [HttpGet("totalExpenses")]
    public async Task<IActionResult> TotalExpenses([FromQuery] GetTotalExpensesQuery request)
    {
        var result = await Dispatcher.DispatchQueryAsync(request);
        return Ok(result.Data);
    }

    [HttpGet("currentIncomes")]
    public async Task<IActionResult> CurrentIncomes([FromQuery] GetCurrentIncomesQuery request)
    {
        var result = await Dispatcher.DispatchQueryAsync(request);
        return Ok(result.Data);
    }

    [HttpGet("currentInvestments")]
    public async Task<IActionResult> CurrentInvestments([FromQuery] GetCurrentInvestmentsQuery request)
    {
        var result = await Dispatcher.DispatchQueryAsync(request);
        return Ok(result.Data);
    }

    [HttpGet("general")]
    public async Task<IActionResult> General([FromQuery] GetGeneralSummaryQuery request)
    {
        var result = await Dispatcher.DispatchQueryAsync(request);
        return Ok(result.Data);
    }
}
