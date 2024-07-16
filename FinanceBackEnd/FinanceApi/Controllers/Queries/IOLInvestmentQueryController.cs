using AutoMapper;
using FinanceApi.Application.Dtos.IOLInvestments;
using FinanceApi.Application.Queries.IOLInvestments;
using FinanceApi.Controllers.Base;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Queries;

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
