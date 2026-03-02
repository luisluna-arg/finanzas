using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Api.Controllers.Requests;
using Finance.Application.Auth;
using Finance.Application.Commands.Incomes;
using Finance.Application.Commands.Incomes.Owners;
using Finance.Application.Dtos.Incomes;
using Finance.Application.Mapping;
using Finance.Application.Services.Interfaces;
using Finance.Application.Services.Orchestrators.IncomePermissionsOrchestrations;
using Finance.Application.Services.Requests.Incomes;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Incomes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/incomes")]
public class IncomeCommandController(
    IMappingService mapper,
    IDispatcher<FinanceDispatchContext> dispatcher,
    IResourcePermissionsSagaService<IncomePermissions, IncomePermissionsOrchestrator, SetIncomeOwnerSagaRequest, DataResult<IncomePermissions>, DeleteIncomeOwnerSagaRequest, CommandResult> fundPermissionsOwnerService,
    ISagaService<CreateIncomeSagaRequest, UpdateIncomeSagaRequest, DeleteIncomeSagaRequest, Income> fundService)
    : ApiBaseCommandController<Income?, Guid, IncomeDto>(mapper, dispatcher)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateIncomeSagaRequest command)
    {
        var result = await fundService.Create(command, httpRequest: Request);
        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(MappingService.Map<IncomeDto>(result.Data));
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateIncomeCommand command)
        => await ExecuteAsync(command);

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteIncomesCommand request)
        => await ExecuteAsync(request);

    [Authorize(Roles = "Admin")]
    [HttpPost("{resourceId}/owner/{userId}")]
    public async Task<IActionResult> SetResourcePermissions(SetIncomeOwnerRequest request)
    {
        var result = await fundPermissionsOwnerService.Set(
            new SetIncomeOwnerSagaRequest(request.ResourceId),
            httpRequest: Request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(MappingService.Map<IncomeDto>(result.Data));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{resourceId}/owner/{userId}")]
    public async Task<IActionResult> DeleteResourcePermissions(DeleteIncomeOwnerRequest request)
    {
        var result = await fundPermissionsOwnerService.Delete(
            new DeleteIncomeOwnerSagaRequest(request.ResourceId));

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok();
    }
}
