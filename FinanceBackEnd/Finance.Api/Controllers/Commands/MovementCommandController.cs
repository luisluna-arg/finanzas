using System.ComponentModel;
using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Commands.Movements;
using Finance.Application.Dtos.Movements;
using Finance.Application.Helpers;
using Finance.Application.Mapping;
using Finance.Domain.Models.Movements;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/movements")]
public class MovementCommandController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : ApiBaseCommandController<Movement?, Guid, MovementDto>(mapper, dispatcher)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateMovementCommand command)
        => await ExecuteAsync(command);

    [HttpPut]
    public async Task<IActionResult> Update(PartialUpdateMovementCommand command)
        => await ExecuteAsync(command);

    [HttpPatch("activate/{id}")]
    public async Task<IActionResult> Activate(Guid id)
        => await ExecuteAsync(new ActivateMovementCommand { Id = id });

    [HttpPatch("deactivate/{id}")]
    public async Task<IActionResult> Deactivate(Guid id)
        => await Handle404Nullable(new DeactivateMovementCommand { Id = id });

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteMovementsCommand request)
        => await ExecuteAsync(request);

    [HttpPost]
    [Route("upload")]
    public async Task<IActionResult> Upload(IFormFile file, Guid appModuleId, Guid bankId, [DefaultValue("Local")] string dateKind)
    {
        await ExecuteAsync(new UploadMovementsFileCommand(file, appModuleId, bankId, EnumHelper.Parse<DateTimeKind>(dateKind)));
        return Ok();
    }
}
