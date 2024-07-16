using AutoMapper;
using FinanceApi.Application.Dtos.CreditCards;
using FinanceApi.Application.Queries.CreditCards;
using FinanceApi.Controllers.Base;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Queries;

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
