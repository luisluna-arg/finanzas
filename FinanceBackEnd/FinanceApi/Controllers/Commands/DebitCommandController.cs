using System.ComponentModel;
using AutoMapper;
using FinanceApi.Application.Commands.Debits;
using FinanceApi.Application.Dtos.Debits;
using FinanceApi.Controllers.Base;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Commands;

public abstract class DebitCommandController(IMapper mapper, IMediator mediator)
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
