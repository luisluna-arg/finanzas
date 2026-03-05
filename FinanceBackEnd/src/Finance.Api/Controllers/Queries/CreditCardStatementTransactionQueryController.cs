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

[Route("api/credit-card-statement-transactions")]
[Authorize(Policy = "AdminOrOwnerPolicy")]
public class CreditCardStatementTransactionQueryController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : ApiBaseQueryController<CreditCardTransaction, Guid, CreditCardTransactionDto>(mapper, dispatcher)
{
    [HttpGet("latest")]
    public async Task<IActionResult> GetLatest([FromQuery] GetLatestCreditCardTransactionsFromStatementsQuery query)
        => await ExecuteAsync(query);
}
