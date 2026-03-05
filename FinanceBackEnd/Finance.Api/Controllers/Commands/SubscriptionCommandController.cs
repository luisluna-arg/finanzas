using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Api.Controllers.Requests;
using Finance.Application.Auth;
using Finance.Application.Legacy.Dtos.Subscriptions;
using Finance.Application.Legacy.Mapping;
using Finance.Application.Legacy.Services.Interfaces;
using Finance.Application.Legacy.Services.Orchestrators.SubscriptionPermissionsOrchestrations;
using Finance.Application.Legacy.Services.Requests.Subscriptions;
using Finance.Application.Legacy.Services.Requests.Subscriptions.Owners;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Subscriptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/Subscriptions")]
public class SubscriptionCommandController(
    IMappingService mapper,
    IDispatcher<FinanceDispatchContext> dispatcher,
    IResourcePermissionsSagaService<SubscriptionPermissions, SubscriptionPermissionsOrchestrator, SetSubscriptionOwnerSagaRequest, DataResult<SubscriptionPermissions>, DeleteSubscriptionOwnerSagaRequest, CommandResult> subscriptionPermissionsOwnerService,
    ISagaService<CreateSubscriptionSagaRequest, UpdateSubscriptionSagaRequest, DeleteSubscriptionSagaRequest, Subscription> subscriptionService)
    : ApiBaseCommandController<Subscription?, Guid, SubscriptionDto>(mapper, dispatcher)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateSubscriptionSagaRequest command)
    {
        var result = await subscriptionService.Create(command, httpRequest: Request);
        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(MappingService.Map<SubscriptionDto>(result.Data));
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateSubscriptionSagaRequest command)
    {
        var result = await subscriptionService.Update(command, httpRequest: Request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(MappingService.Map<SubscriptionDto>(result.Data));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteSubscriptionSagaRequest command)
    {
        var result = await subscriptionService.Delete(command, httpRequest: Request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{resourceId}/owner/{userId}")]
    public async Task<IActionResult> SetResourcePermissions(SetSubscriptionOwnerRequest request)
    {
        var result = await subscriptionPermissionsOwnerService.Set(
            new SetSubscriptionOwnerSagaRequest(request.ResourceId),
            httpRequest: Request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(MappingService.Map<SubscriptionDto>(result.Data));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{resourceId}/owner/{userId}")]
    public async Task<IActionResult> DeleteResourcePermissions(DeleteSubscriptionOwnerRequest request)
    {
        var result = await subscriptionPermissionsOwnerService.Delete(
            new DeleteSubscriptionOwnerSagaRequest(request.ResourceId));

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok();
    }
}
