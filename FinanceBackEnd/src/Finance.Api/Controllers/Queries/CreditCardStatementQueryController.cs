using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Legacy.Dtos.CreditCards;
using Finance.Application.Legacy.Mapping;
using Finance.Application.Legacy.Queries.CreditCards;
using Finance.Domain.Models.CreditCards;
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
