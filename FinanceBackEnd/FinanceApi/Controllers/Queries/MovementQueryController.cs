using AutoMapper;
using FinanceApi.Application.Dtos.Movements;
using FinanceApi.Application.Queries.Movements;
using FinanceApi.Controllers.Base;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Queries;

[Route("api/movements")]
public class MovementQueryController(IMapper mapper, IMediator mediator)
    : ApiBaseQueryController<Movement?, Guid, MovementDto>(mapper, mediator)
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetMovementsQuery request)
        => await Handle(request);

    [HttpGet("paginated")]
    public async Task<IActionResult> GetPaginated([FromQuery] GetPaginatedMovementsQuery request)
        => await Handle(request);

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromQuery] GetSingleMovementQuery request)
        => await Handle(request);

    [HttpGet("latest/{appModuleId}")]
    public async Task<IActionResult> Latest(Guid appModuleId)
        => await Handle(new GetLatestMovementQuery(appModuleId));
}
