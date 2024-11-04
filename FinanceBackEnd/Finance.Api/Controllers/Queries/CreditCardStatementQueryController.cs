using AutoMapper;
using Finance.Api.Controllers.Base;
using Finance.Application.Dtos.CreditCards;
using Finance.Application.Queries.CreditCards;
using Finance.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/credit-card-statements")]
public class CreditCardStatementQueryController(IMapper mapper, IMediator mediator)
    : ApiBaseQueryController<CreditCardStatement, Guid, CreditCardStatementDto>(mapper, mediator)
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetCreditCardStatementsQuery request)
        => await Handle(request);

    [HttpGet("paginated")]
    public async Task<IActionResult> GetPaginated([FromQuery] GetPaginatedCreditCardStatementsQuery request)
        => await Handle(request);

    [HttpGet]
    [Route("latest")]
    public async Task<IActionResult> Latest([FromQuery] GetLatestCreditCardStatementsQuery request)
        => await Handle(request);
}
