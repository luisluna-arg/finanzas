using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Commands.CreditCards;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/credit-card-payment-plans")]
[Authorize(Policy = "AdminOrOwnerPolicy")]
public class CreditCardPaymentPlanCommandController(IDispatcher<FinanceDispatchContext> dispatcher)
    : SecuredApiController
{
    [HttpPost("backfill")]
    public async Task<IActionResult> Backfill()
    {
        var result = await dispatcher.DispatchCommandAsync(new BackfillCreditCardPaymentPlansCommand());
        return result.IsSuccess ? Ok() : BadRequest(result.ErrorMessage);
    }
}
