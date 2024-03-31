using AutoMapper;
using FinanceApi.Application.Dtos.Banks;
using FinanceApi.Application.Queries.Summary;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers;

[Route("api/summary")]
public class SummaryController : ApiBaseController<Bank?, Guid, BankDto>
{
    public SummaryController(IMapper mapper, IMediator mediator)
        : base(mapper, mediator)
    {
    }

    [HttpGet("currentFunds")]
    public async Task<IActionResult> TotalFunds([FromQuery] GetCurrentFundsQuery request)
        => Ok(await mediator.Send(request));

    [HttpGet("totalExpenses")]
    public async Task<IActionResult> TotalExpenses([FromQuery] GetTotalExpensesQuery request)
        => Ok(await mediator.Send(request));

    [HttpGet("currentInvestments")]
    public async Task<IActionResult> CurrentInvestments([FromQuery] GetCurrentInvestmentsQuery request)
        => Ok(await mediator.Send(request));
}
