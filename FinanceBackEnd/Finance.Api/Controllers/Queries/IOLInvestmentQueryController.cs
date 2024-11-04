using AutoMapper;
using Finance.Api.Controllers.Base;
using Finance.Application.Dtos.IOLInvestments;
using Finance.Application.Queries.IOLInvestments;
using Finance.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/iol-investment")]
public class IOLInvestmentQueryController(IMapper mapper, IMediator mediator)
    : ApiBaseQueryController<IOLInvestment?, Guid, IOLInvestmentDto>(mapper, mediator)
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetIOLInvestmentsQuery request)
        => await Handle(request);

    [HttpGet("paginated")]
    public async Task<IActionResult> GetPaginated([FromQuery] GetPaginatedIOLInvestmentsQuery request)
        => await Handle(request);

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromQuery] GetSingleIOLInvestmentQuery request)
        => await Handle(request);
}
