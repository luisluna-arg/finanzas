using AutoMapper;
using Finance.Api.Controllers.Base;
using Finance.Application.Dtos.Movements;
using Finance.Application.Queries.Movements;
using Finance.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

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
