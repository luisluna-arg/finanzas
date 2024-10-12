using System.ComponentModel;
using AutoMapper;
using FinanceApi.Application.Commands.Debits;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Commands;

[Route("api/debits/annual")]
public class DebitAnnualCommandController(IMapper mapper, IMediator mediator)
    : DebitCommandController(mapper, mediator)
{
    [HttpPost]
    public new async Task<IActionResult> Create(CreateDebitCommand command)
    {
        command.Frequency = FrequencyEnum.Annual;
        return await base.Create(command);
    }

    [HttpPost]
    [Route("upload")]
    public async Task<IActionResult> Upload(IFormFile file, string appModuleId, [DefaultValue("Local")] string dateKind)
    {
        await Handle(new UploadDebitsFileCommand(file, appModuleId, EnumHelper.Parse<DateTimeKind>(dateKind), FrequencyEnum.Annual));
        return Ok();
    }
}
