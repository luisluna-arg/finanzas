using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Legacy.Commands.CreditCards;
using Finance.Application.Legacy.Dtos.CreditCards;
using Finance.Application.Legacy.Mapping;
using Finance.Domain.Models.CreditCards;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/credit-card-payments")]
public class CreditCardPaymentCommandController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : ApiBaseCommandController<CreditCardPayment, Guid, CreditCardPaymentDto>(mapper, dispatcher)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateCreditCardPaymentCommand request)
        => await ExecuteAsync(request);

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteCreditCardPaymentCommand request)
        => await ExecuteAsync(request);
}
