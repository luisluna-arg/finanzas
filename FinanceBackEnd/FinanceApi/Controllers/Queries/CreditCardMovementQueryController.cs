using AutoMapper;
using FinanceApi.Application.Dtos.CreditCards;
using FinanceApi.Application.Queries.CreditCards;
using FinanceApi.Controllers.Base;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Queries;

[Route("api/credit-card-movements")]
public class CreditCardMovementQueryController(IMapper mapper, IMediator mediator)
    : ApiBaseQueryController<CreditCardMovement, Guid, CreditCardMovementDto>(mapper, mediator)
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetCreditCardMovementsQuery request)
        => await Handle(request);

    [HttpGet("paginated")]
    public async Task<IActionResult> GetPaginated([FromQuery] GetPaginatedCreditCardMovementsQuery request)
        => await Handle(request);

    [HttpGet]
    [Route("latest")]
    public async Task<IActionResult> Latest([FromQuery] GetLatestCreditCardMovementsQuery request)
        => await Handle(request);
}
