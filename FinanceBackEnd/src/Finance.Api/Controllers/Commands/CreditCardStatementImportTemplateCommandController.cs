using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Commands.CreditCards;
using Finance.Application.Dtos.CreditCards;
using Finance.Application.Mapping;
using Finance.Domain.Models.CreditCards;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/credit-card-statement-import-templates")]
[Authorize(Policy = "AdminOrOwnerPolicy")]
public class CreditCardStatementImportTemplateCommandController(
    IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : ApiBaseCUDCommandController<
        CreditCardStatementImportTemplate,
        Guid,
        CreditCardStatementImportTemplateDto,
        CreateCreditCardStatementImportTemplateCommand,
        UpdateCreditCardStatementImportTemplateCommand,
        DeleteCreditCardStatementImportTemplateCommand>(mapper, dispatcher)
{
    [HttpPost("{templateId}/associate")]
    public async Task<IActionResult> Associate(Guid templateId, [FromBody] AssociateCreditCardImportTemplateBody body)
    {
        var command = new AssociateCreditCardImportTemplateCommand
        {
            TemplateId = templateId,
            CreditCardId = body.CreditCardId,
        };
        await Dispatcher.DispatchCommandAsync(command);
        return Ok();
    }
}

public record AssociateCreditCardImportTemplateBody(Guid CreditCardId);
