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

[Route("api/credit-card-transactions")]
[Authorize(Policy = "AdminOrOwnerPolicy")]
public class CreditCardTransactionQueryController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : ApiBaseQueryController<CreditCardTransaction, Guid, CreditCardTransactionDto>(mapper, dispatcher)
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetCreditCardTransactionsQuery query)
        => await ExecuteAsync(query);
}
