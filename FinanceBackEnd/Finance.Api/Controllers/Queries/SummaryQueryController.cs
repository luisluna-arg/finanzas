using AutoMapper;
using Finance.Api.Controllers.Base;
using Finance.Application.Dtos.Banks;
using Finance.Application.Queries.Summary;
using Finance.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/summary")]
public class SummaryQueryController(IMapper mapper, IMediator mediator)
    : ApiBaseQueryController<Bank?, Guid, BankDto>(mapper, mediator)
{
    [HttpGet("currentFunds")]
    public async Task<IActionResult> TotalFunds([FromQuery] GetCurrentFundsQuery request)
        => Ok(await Mediator.Send(request));

    [HttpGet("totalExpenses")]
    public async Task<IActionResult> TotalExpenses([FromQuery] GetTotalExpensesQuery request)
        => Ok(await Mediator.Send(request));

    [HttpGet("currentIncomes")]
    public async Task<IActionResult> CurrentIncomes([FromQuery] GetCurrentIncomesQuery request)
        => Ok(await Mediator.Send(request));

    [HttpGet("currentInvestments")]
    public async Task<IActionResult> CurrentInvestments([FromQuery] GetCurrentInvestmentsQuery request)
        => Ok(await Mediator.Send(request));

    [HttpGet("general")]
    public async Task<IActionResult> General([FromQuery] GetGeneralSummaryQuery request)
        => Ok(await Mediator.Send(request));
}
