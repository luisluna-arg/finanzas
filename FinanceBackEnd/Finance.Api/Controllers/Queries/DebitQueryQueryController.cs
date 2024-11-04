using AutoMapper;
using Finance.Api.Controllers.Base;
using Finance.Application.Dtos.Debits;
using Finance.Application.Queries.Debits;
using Finance.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/debits/{frequency}")]
public class DebitQueryController(IMapper mapper, IMediator mediator)
    : ApiBaseQueryController<Debit?, Guid, DebitDto>(mapper, mediator)
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetAllDebitsQuery request)
        => await Handle(request);

    [HttpGet]
    [Route("latest")]
    public async Task<IActionResult> Latest([FromQuery] GetLatestDebitsQuery request)
        => await Handle(request);

    [HttpGet("paginated")]
    public async Task<IActionResult> GetPaginated([FromQuery] GetPaginatedDebitsQuery request)
        => await Handle(request);
}
