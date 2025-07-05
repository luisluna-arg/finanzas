using System.ComponentModel;
using Finance.Api.Controllers.Base;
using Finance.Application.Commands.Debits;
using Finance.Application.Dtos.Debits;
using Finance.Application.Helpers;
using Finance.Application.Mapping;
using Finance.Domain.Enums;
using Finance.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

public abstract class DebitCommandController(IMappingService mapper, IMediator mediator)
    : ApiBaseCommandController<Debit?, Guid, DebitDto>(mapper, mediator)
{
    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteDebitCommand request)
        => await Handle(request);

    [HttpPatch("activate/{id}")]
    public async Task<IActionResult> Activate(Guid id)
        => await Handle(new ActivateDebitCommand { Id = id });

    [HttpPatch("deactivate/{id}")]
    public async Task<IActionResult> Deactivate(Guid id)
        => await Handle404(new DeactivateDebitCommand { Id = id });

    [HttpPost]
    protected async Task<IActionResult> Create(CreateDebitCommand command)
        => await Handle(command);

    [HttpPost]
    [Route("upload")]
    protected async Task<IActionResult> Upload(IFormFile file, string appModuleId, [DefaultValue("Local")] string dateKind, FrequencyEnum frequency)
    {
        await Handle(new UploadDebitsFileCommand(file, appModuleId, EnumHelper.Parse<DateTimeKind>(dateKind), frequency));
        return Ok();
    }
}
