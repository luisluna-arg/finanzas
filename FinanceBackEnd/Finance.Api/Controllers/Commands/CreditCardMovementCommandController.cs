using System.ComponentModel;
using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Commands.CreditCards;
using Finance.Application.Dtos.CreditCards;
using Finance.Application.Helpers;
using Finance.Application.Mapping;
using Finance.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/credit-card-transactions")]
public class CreditCardTransactionCommandController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : ApiBaseCommandController<CreditCardTransaction, Guid, CreditCardTransactionDto>(mapper, dispatcher)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateCreditCardTransactionCommand request)
        => await ExecuteAsync(request);

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteCreditCardTransactionCommand request)
        => await ExecuteAsync(request);

    [HttpPost]
    [Route("upload")]
    public async Task<IActionResult> Upload(IFormFile file, string creditCardId, [DefaultValue("Local")] string dateKind)
    {
        await ExecuteAsync(new UploadCreditCardTransactionFileCommand(file, creditCardId, EnumHelper.Parse<DateTimeKind>(dateKind)));
        return Ok();
    }
}
