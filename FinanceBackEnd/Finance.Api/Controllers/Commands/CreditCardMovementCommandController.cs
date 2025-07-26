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

[Route("api/credit-card-movements")]
public class CreditCardMovementCommandController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : ApiBaseCommandController<CreditCardMovement, Guid, CreditCardMovementDto>(mapper, dispatcher)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateCreditCardMovementCommand request)
        => await ExecuteAsync(request);

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteCreditCardMovementCommand request)
        => await ExecuteAsync(request);

    [HttpPost]
    [Route("upload")]
    public async Task<IActionResult> Upload(IFormFile file, string creditCardId, [DefaultValue("Local")] string dateKind)
    {
        await ExecuteAsync(new UploadCreditCardFileCommand(file, creditCardId, EnumHelper.Parse<DateTimeKind>(dateKind)));
        return Ok();
    }
}
