using System.ComponentModel;
using AutoMapper;
using FinanceApi.Application.Commands.Debits;
using FinanceApi.Application.Dtos.Debits;
using FinanceApi.Controllers.Base;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Commands;

[Route("api/debits")]
public class DebitCommandController(IMapper mapper, IMediator mediator)
    : ApiBaseCommandController<Debit?, Guid, DebitDto>(mapper, mediator)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateDebitCommand command)
        => await Handle(command);

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteDebitCommand request)
        => await Handle(request);

    [HttpPost]
    [Route("upload")]
    public async Task<IActionResult> Upload(IFormFile file, string appModuleId, [DefaultValue("Local")] string dateKind)
    {
        await Handle(new UploadDebitsFileCommand(file, appModuleId, EnumHelper.Parse<DateTimeKind>(dateKind)));
        return Ok();
    }

    [HttpPatch("activate/{id}")]
    public async Task<IActionResult> Activate(Guid id)
        => await Handle(new ActivateDebitCommand { Id = id });

    [HttpPatch("deactivate/{id}")]
    public async Task<IActionResult> Deactivate(Guid id)
        => await Handle404(new DeactivateDebitCommand { Id = id });
}
