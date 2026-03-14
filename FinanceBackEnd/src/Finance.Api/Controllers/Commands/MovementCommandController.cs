using System.ComponentModel;
using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Api.Controllers.Requests;
using Finance.Application.Auth;
using Finance.Application.Commands.Movements;
using Finance.Application.Dtos.Movements;
using Finance.Application.Helpers;
using Finance.Application.Mapping;
using Finance.Application.Services;
using Finance.Application.Services.Movements;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Movements;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/movements")]
public class MovementCommandController(
    IMappingService mapper,
    IDispatcher<FinanceDispatchContext> dispatcher,
    MovementService movementService)
    : CommandController<
        Movement,
        MovementPermissions,
        CreateMovementRequest,
        UpdateMovementRequest,
        DeleteMovementRequest,
        SetMovementOwnerRequest,
        DeleteMovementOwnerRequest,
        Guid,
        MovementDto,
        MovementService>(mapper, dispatcher, movementService)
{
    [HttpPost]
    [Route("upload")]
    public async Task<IActionResult> Upload(IFormFile file, Guid appModuleId, Guid bankId, [DefaultValue("Local")] string dateKind)
    {
        await ExecuteAsync(new UploadMovementsFileCommand(file, appModuleId, bankId, EnumHelper.Parse<DateTimeKind>(dateKind)));
        return Ok();
    }
}
