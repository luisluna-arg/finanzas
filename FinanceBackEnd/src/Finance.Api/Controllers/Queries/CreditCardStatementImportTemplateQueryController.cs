using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Dtos.CreditCards;
using Finance.Application.Mapping;
using Finance.Application.Queries.CreditCards;
using Finance.Domain.Models.CreditCards;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/credit-card-statement-import-templates")]
[Authorize(Policy = "AdminOrOwnerPolicy")]
public class CreditCardStatementImportTemplateQueryController(
    IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : ApiBaseQueryController<CreditCardStatementImportTemplate, Guid, CreditCardStatementImportTemplateDto>(mapper, dispatcher)
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetCreditCardStatementImportTemplatesQuery request)
        => await ExecuteAsync(request!);
}
