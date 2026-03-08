using System.ComponentModel;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Helpers;
using Finance.Application.Legacy.Commands.Debits;
using Finance.Application.Legacy.Mapping;
using Finance.Application.Services;
using Finance.Application.Services.Debits;
using Finance.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/debits/annual")]
public class DebitAnnualCommandController(
    IMappingService mapper,
    IDispatcher<FinanceDispatchContext> dispatcher,
    DebitService debitService)
    : DebitCommandController(mapper, dispatcher, debitService)
{
    [HttpPost]
    public new async Task<IActionResult> Create(CreateDebitRequest request)
        => await base.Create(request with { Frequency = FrequencyEnum.Annual });

    [HttpPost]
    [Route("upload")]
    public async Task<IActionResult> Upload(IFormFile file, string appModuleId, [DefaultValue("Local")] string dateKind)
    {
        await ExecuteAsync(new UploadDebitsFileCommand(file, appModuleId, EnumHelper.Parse<DateTimeKind>(dateKind), FrequencyEnum.Annual));
        return Ok();
    }
}
