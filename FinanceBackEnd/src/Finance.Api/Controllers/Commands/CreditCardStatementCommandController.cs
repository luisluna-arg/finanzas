using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Commands.CreditCards;
using Finance.Application.Dtos.CreditCards;
using Finance.Application.Mapping;
using Finance.Domain.Models.CreditCards;
using Finance.Helpers.ExcelHelper;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/credit-card-statements")]
public class CreditCardStatementCommandController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : ApiBaseCUDCommandController<
    CreditCardStatement?,
    Guid,
    CreditCardStatementDto,
    CreateCreditCardStatementCommand,
    UpdateCreditCardStatementCommand,
    DeleteCreditCardStatementCommand
    >(mapper, dispatcher)
{
    public override async Task<IActionResult> Create(CreateCreditCardStatementCommand command)
    {
        var result = await Dispatcher.DispatchAsync<DataResult<CreditCardStatement>>(command);
        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);
        return Ok(MappingService.Map<CreditCardStatementDto>(result.Data!));
    }

    [HttpPost("import")]
    public async Task<IActionResult> Import(IFormFile file, [FromForm] string templateId, [FromForm] string statementId)
    {
        var command = new ImportCreditCardStatementTransactionsCommand
        {
            File = file,
            TemplateId = new Guid(templateId),
            StatementId = new Guid(statementId),
        };
        return await ExecuteAsync(command);
    }

    [HttpPost("pdf-diagnose")]
    public IActionResult PdfDiagnose(IFormFile file)
    {
        var result = new StatementImportPdfHelper().Diagnose(file);
        return Ok(result);
    }
}
