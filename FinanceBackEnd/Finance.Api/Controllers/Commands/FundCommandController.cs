using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Api.Controllers.Requests;
using Finance.Application.Auth;
using Finance.Application.Commands.FundOwners;
using Finance.Application.Commands.Funds;
using Finance.Application.Dtos.Funds;
using Finance.Application.Mapping;
using Finance.Application.Services.Interfaces;
using Finance.Application.Services.Requests.Funds;
using Finance.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/funds")]
public class FundCommandController(
    IMappingService mapper,
    IDispatcher<FinanceDispatchContext> dispatcher,
    IResourceOwnerSagaService<SetFundOwnerSagaRequest, DeleteFundOwnerSagaRequest, FundResource> fundResourceOwnerService,
    ISagaService<CreateFundSagaRequest, UpdateFundSagaRequest, DeleteFundSagaRequest, Fund> fundService)
    : ApiBaseCommandController<Fund?, Guid, FundDto>(mapper, dispatcher)
{
    private ISagaService<CreateFundSagaRequest, UpdateFundSagaRequest, DeleteFundSagaRequest, Fund> FundService { get => fundService; }
    private IResourceOwnerSagaService<SetFundOwnerSagaRequest, DeleteFundOwnerSagaRequest, FundResource> FundResourceOwnerService { get => fundResourceOwnerService; }

    [HttpPost]
    public async Task<IActionResult> Create(CreateFundSagaRequest command)
    {
        var result = await FundService.Create(command, httpRequest: Request);
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
    [HttpPost("{bankId}/owner/{userId}")]
    public async Task<IActionResult> SetResourceOwner(SetFundOwnerRequest request)
    {
        var result = await FundResourceOwnerService.Set(
            new SetFundOwnerSagaRequest(request.FundId),
            httpRequest: Request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(MappingService.Map<FundDto>(result.Data));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{bankId}/owner/{userId}")]
    public async Task<IActionResult> DeleteResourceOwner(DeleteFundOwnerRequest request)
    {
        var result = await FundResourceOwnerService.Delete(
            new DeleteFundOwnerSagaRequest(
                request.FundId,
                request.UserId));

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok();
    }
}