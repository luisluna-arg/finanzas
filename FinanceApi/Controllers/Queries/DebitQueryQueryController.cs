using AutoMapper;
using FinanceApi.Application.Dtos.Debits;
using FinanceApi.Application.Queries.Debits;
using FinanceApi.Controllers.Base;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Queries;

[Route("api/debits")]
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
