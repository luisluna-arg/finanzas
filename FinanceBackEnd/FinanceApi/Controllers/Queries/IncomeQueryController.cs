using AutoMapper;
using FinanceApi.Application.Dtos.Incomes;
using FinanceApi.Application.Queries.Incomes;
using FinanceApi.Controllers.Base;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Queries;

[Route("api/incomes")]
public class IncomeQueryController(IMapper mapper, IMediator mediator)
    : ApiBaseQueryController<Income?, Guid, IncomeDto>(mapper, mediator)
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetIncomesQuery request)
        => await Handle(request);

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromQuery] GetSingleIncomeQuery request)
        => await Handle(request);

    [HttpGet("paginated")]
    public async Task<IActionResult> GetPaginated([FromQuery] GetPaginatedIncomesQuery request)
        => await Handle(request);

    [HttpGet("latest/{appModuleId}")]
    public async Task<IActionResult> Latest(Guid appModuleId)
        => await Handle(new GetLatestIncomeQuery(appModuleId));
}