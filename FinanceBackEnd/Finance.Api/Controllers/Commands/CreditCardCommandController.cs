using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Commands.CreditCards;
using Finance.Application.Dtos.CreditCards;
using Finance.Application.Mapping;
using Finance.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/credit-cards")]
public class CreditCardCommandController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : ApiBaseCommandController<CreditCard?, Guid, CreditCardDto>(mapper, dispatcher)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateCreditCardCommand command)
    => await ExecuteAsync(command);

    [HttpPut]
    public async Task<IActionResult> Update(UpdateCreditCardCommand command)
        => await ExecuteAsync(command);

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteCreditCardCommand request)
        => await ExecuteAsync(request);
}
