using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Dtos.CreditCards;
using Finance.Application.Mapping;
using Finance.Application.Queries.CreditCards;
using Finance.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/credit-card-movements")]
public class CreditCardMovementQueryController(IMappingService mapper, IDispatcher dispatcher)
    : ApiBaseQueryController<CreditCardMovement, Guid, CreditCardMovementDto>(mapper, dispatcher)
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetCreditCardMovementsQuery request)
        => await ExecuteAsync(request);

    [HttpGet("paginated")]
    public async Task<IActionResult> GetPaginated([FromQuery] GetPaginatedCreditCardMovementsQuery request)
        => await ExecuteAsync(request);

    [HttpGet]
    [Route("latest")]
    public async Task<IActionResult> Latest([FromQuery] GetLatestCreditCardMovementsQuery request)
        => await ExecuteAsync(request);
}
