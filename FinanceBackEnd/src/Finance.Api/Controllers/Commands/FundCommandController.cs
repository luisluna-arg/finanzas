using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Api.Controllers.Requests;
using Finance.Application.Auth;
using Finance.Application.Legacy.Commands.Funds;
using Finance.Application.Legacy.Commands.Funds.Owners;
using Finance.Application.Legacy.Dtos.Funds;
using Finance.Application.Legacy.Mapping;
using Finance.Application.Legacy.Services.Interfaces;
using Finance.Application.Legacy.Services.Orchestrators.FundPermissionsOrchestrations;
using Finance.Application.Legacy.Services.Requests.Funds;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Funds;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/funds")]
public class FundCommandController(
    IMappingService mapper,
    IDispatcher<FinanceDispatchContext> dispatcher,
    IResourcePermissionsSagaService<FundPermissions, FundPermissionsOrchestrator, SetFundOwnerSagaRequest, DataResult<FundPermissions>, DeleteFundOwnerSagaRequest, CommandResult> fundPermissionsOwnerService,
    ISagaService<CreateFundSagaRequest, UpdateFundSagaRequest, DeleteFundSagaRequest, Fund> fundService)
    : ApiBaseCommandController<Fund?, Guid, FundDto>(mapper, dispatcher)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateFundSagaRequest command)
    {
        var result = await fundService.Create(command, httpRequest: Request);
        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(MappingService.Map<FundDto>(result.Data));
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateFundCommand command)
        => await ExecuteAsync(command);

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteFundsCommand request)
        => await ExecuteAsync(request);

    [Authorize(Roles = "Admin")]
    [HttpPost("{resourceId}/owner/{userId}")]
    public async Task<IActionResult> SetResourcePermissions(SetFundOwnerRequest request)
    {
        var result = await fundPermissionsOwnerService.Set(
            new SetFundOwnerSagaRequest(request.ResourceId),
            httpRequest: Request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(MappingService.Map<FundDto>(result.Data));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{resourceId}/owner/{userId}")]
    public async Task<IActionResult> DeleteResourcePermissions(DeleteFundOwnerRequest request)
    {
        var result = await fundPermissionsOwnerService.Delete(
            new DeleteFundOwnerSagaRequest(request.ResourceId));

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok();
    }
}
