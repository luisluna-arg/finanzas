using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Api.Controllers.Requests;
using Finance.Application.Auth;
using Finance.Application.Commands.CreditCards;
using Finance.Application.Dtos.CreditCards;
using Finance.Application.Mapping;
using Finance.Application.Services;
using Finance.Application.Services.CreditCards;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.CreditCards;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/credit-cards")]
public class CreditCardCommandController(
    IMappingService mapper,
    IDispatcher<FinanceDispatchContext> dispatcher,
    CreditCardService creditCardService)
    : CommandController<
        CreditCard,
        CreditCardPermissions,
        CreateCreditCardRequest,
        UpdateCreditCardRequest,
        DeleteCreditCardRequest,
        SetCreditCardOwnerRequest,
        DeleteCreditCardOwnerRequest,
        Guid,
        CreditCardDto,
        CreditCardService>(mapper, dispatcher, creditCardService)
{
    [HttpPatch("{id}/default-import-template")]
    public async Task<IActionResult> SetDefaultImportTemplate(Guid id, [FromBody] SetDefaultImportTemplateRequest request)
    {
        await ExecuteAsync(new SetCreditCardDefaultImportTemplateCommand
        {
            CreditCardId = id,
            TemplateId = request.TemplateId,
        });
        return Ok();
    }
}

public record SetDefaultImportTemplateRequest(Guid? TemplateId);
