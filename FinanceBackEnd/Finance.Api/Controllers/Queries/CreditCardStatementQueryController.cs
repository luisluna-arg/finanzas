using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Dtos.CreditCards;
using Finance.Application.Mapping;
using Finance.Application.Queries.CreditCards;
using Finance.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/credit-card-statements")]
[Authorize(Policy = "AdminOrOwnerPolicy")]
public class CreditCardStatementQueryController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : ApiBaseQueryController<CreditCardStatement, Guid, CreditCardStatementDto>(mapper, dispatcher)
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetCreditCardStatementsQuery request)
        => await ExecuteAsync(request!);

    [HttpGet("paginated")]
    public async Task<IActionResult> GetPaginated([FromQuery] GetPaginatedCreditCardStatementsQuery request)
        => await ExecuteAsync(request);

    [HttpGet]
    [Route("latest")]
    public async Task<IActionResult> Latest([FromQuery] GetLatestCreditCardStatementsQuery request)
        => await ExecuteAsync(request);
}
