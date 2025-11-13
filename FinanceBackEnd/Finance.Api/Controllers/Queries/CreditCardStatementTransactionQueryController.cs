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

[Route("api/credit-card-statement-transactions")]
[Authorize(Policy = "AdminOrOwnerPolicy")]
public class CreditCardStatementTransactionQueryController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : ApiBaseQueryController<CreditCardTransaction, Guid, CreditCardTransactionDto>(mapper, dispatcher)
{
    [HttpGet("latest")]
    public async Task<IActionResult> GetLatest([FromQuery] GetLatestCreditCardTransactionsFromStatementsQuery query)
        => await ExecuteAsync(query);
}