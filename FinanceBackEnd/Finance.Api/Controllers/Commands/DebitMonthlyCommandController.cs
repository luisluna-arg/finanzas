using System.ComponentModel;
using CQRSDispatch.Interfaces;
using Finance.Application.Commands.Debits;
using Finance.Application.Mapping;
using Finance.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/debits/monthly")]
public class MonthlyDebitCommandController(IMappingService mapper, IDispatcher dispatcher)
    : DebitCommandController(mapper, dispatcher)
{
    [HttpPost]
    public new async Task<IActionResult> Create(CreateDebitCommand command)
    {
        command.Frequency = FrequencyEnum.Monthly;
        return await base.Create(command);
    }

    [HttpPost]
    [Route("upload")]
    public async Task<IActionResult> Upload(IFormFile file, string appModuleId, [DefaultValue("Local")] string dateKind)
        => await Upload(file, appModuleId, dateKind, FrequencyEnum.Monthly);
}
