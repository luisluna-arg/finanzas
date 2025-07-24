using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Api.Controllers.Requests;
using Finance.Application.Commands;
using Finance.Application.Commands.Funds;
using Finance.Application.Dtos.Funds;
using Finance.Application.Mapping;
using Finance.Application.Services.Interfaces;
using Finance.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/funds")]
[Authorize(Policy = "AdminOrOwnerPolicy")]
public class FundCommandController(
    IMappingService mapper,
    IDispatcher dispatcher,
    IResourceOwnerSagaService<SetFundOwnerSagaRequest, DeleteFundOwnerSagaRequest, Fund> fundResourceOwnerService)
    : ApiBaseCommandController<Fund?, Guid, FundDto>(mapper, dispatcher)
{
    private IResourceOwnerSagaService<SetFundOwnerSagaRequest, DeleteFundOwnerSagaRequest, Fund> FundResourceOwnerService { get => fundResourceOwnerService; }

    [HttpPost]
    public async Task<IActionResult> Create(CreateFundCommand command)
        => await ExecuteAsync(command);

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
        var (result, _) = await FundResourceOwnerService.Set(
            new SetFundOwnerSagaRequest(
                request.FundId,
                request.UserId));
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{bankId}/owner/{userId}")]
    public async Task<IActionResult> DeleteResourceOwner(DeleteFundOwnerRequest request)
    {
        await FundResourceOwnerService.Delete(
            new DeleteFundOwnerSagaRequest(
                request.FundId,
                request.UserId));
        return Ok();
    }
}