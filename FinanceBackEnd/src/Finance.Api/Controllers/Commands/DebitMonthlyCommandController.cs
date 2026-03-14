using System.ComponentModel;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Mapping;
using Finance.Application.Services;
using Finance.Application.Services.Debits;
using Finance.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/debits/monthly")]
public class MonthlyDebitCommandController(
    IMappingService mapper,
    IDispatcher<FinanceDispatchContext> dispatcher,
    DebitService debitService)
    : DebitCommandController(mapper, dispatcher, debitService)
{
    [HttpPost]
    public override async Task<IActionResult> Create(CreateDebitRequest request)
        => await base.Create(request with { Frequency = FrequencyEnum.Monthly });

    [HttpPost]
    [Route("upload")]
    public async Task<IActionResult> Upload(IFormFile file, string appModuleId, [DefaultValue("Local")] string dateKind)
        => await Upload(file, appModuleId, dateKind, FrequencyEnum.Monthly);
}
