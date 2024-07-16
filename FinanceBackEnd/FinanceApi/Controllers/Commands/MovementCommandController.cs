using System.ComponentModel;
using AutoMapper;
using FinanceApi.Application.Commands.Movements;
using FinanceApi.Application.Dtos.Movements;
using FinanceApi.Controllers.Base;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Commands;

[Route("api/movements")]
public class MovementCommandController(IMapper mapper, IMediator mediator)
    : ApiBaseCommandController<Movement?, Guid, MovementDto>(mapper, mediator)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateMovementCommand command)
        => await Handle(command);

    [HttpPut]
    public async Task<IActionResult> Update(PartialUpdateMovementCommand command)
        => await Handle(command);

    [HttpPatch("activate/{id}")]
    public async Task<IActionResult> Activate(Guid id)
        => await Handle(new ActivateMovementCommand { Id = id });

    [HttpPatch("deactivate/{id}")]
    public async Task<IActionResult> Deactivate(Guid id)
        => await Handle404(new DeactivateMovementCommand { Id = id });

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteMovementsCommand request)
        => await Handle(request);

    [HttpPost]
    [Route("upload")]
    public async Task<IActionResult> Upload(IFormFile file, Guid appModuleId, Guid bankId, [DefaultValue("Local")] string dateKind)
    {
        await Handle(new UploadMovementsFileCommand(file, appModuleId, bankId, EnumHelper.Parse<DateTimeKind>(dateKind)));
        return Ok();
    }
}
