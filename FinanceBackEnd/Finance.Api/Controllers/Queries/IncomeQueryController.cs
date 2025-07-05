using Finance.Api.Controllers.Base;
using Finance.Application.Dtos.Incomes;
using Finance.Application.Mapping;
using Finance.Application.Queries.Incomes;
using Finance.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/incomes")]
public class IncomeQueryController(IMappingService mapper, IMediator mediator)
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
        => await Handle(request!);

    [HttpGet("latest/{appModuleId}")]
    public async Task<IActionResult> Latest(Guid appModuleId)
        => await Handle(new GetLatestIncomeQuery(appModuleId));
}