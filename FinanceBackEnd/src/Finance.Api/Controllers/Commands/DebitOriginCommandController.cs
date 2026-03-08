using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Api.Controllers.Requests;
using Finance.Application.Auth;
using Finance.Application.Legacy.Dtos.DebitOrigins;
using Finance.Application.Legacy.Mapping;
using Finance.Application.Services;
using Finance.Application.Services.DebitOrigins;
using Finance.Domain.Models.Debits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/debit-origins")]
public class DebitOriginCommandController(
    IMappingService mapper,
    IDispatcher<FinanceDispatchContext> dispatcher,
    DebitOriginService debitOriginService)
    : ApiBaseCommandController<DebitOrigin?, Guid, DebitOriginDto>(mapper, dispatcher)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateDebitOriginRequest request)
    {
        var result = await debitOriginService.Create(request, Request);
        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);
        return Ok(MappingService.Map<DebitOriginDto>(result.Data));
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateDebitOriginRequest request)
    {
        var result = await debitOriginService.Update(request, Request);
        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);
        return Ok(MappingService.Map<DebitOriginDto>(result.Data));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteDebitOriginRequest request)
    {
        var result = await debitOriginService.Delete(request, Request);
        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);
        return Ok();
    }

    [HttpPost]
    [Route("activate")]
    public async Task<IActionResult> Activate(ActivateDebitOriginRequest request)
    {
        var result = await debitOriginService.Activate(request, Request);
        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);
        return Ok();
    }

    [HttpPost]
    [Route("deactivate")]
    public async Task<IActionResult> Deactivate(DeactivateDebitOriginRequest request)
    {
        var result = await debitOriginService.Deactivate(request, Request);
        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);
        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{resourceId}/owner/{userId}")]
    public async Task<IActionResult> SetOwner(SetDebitOriginOwnerRequest request)
    {
        var result = await debitOriginService.SetOwner(request.ResourceId, Request);
        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);
        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{resourceId}/owner/{userId}")]
    public async Task<IActionResult> DeleteOwner(DeleteDebitOriginOwnerRequest request)
    {
        var result = await debitOriginService.DeleteOwner(request.ResourceId, Request);
        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);
        return Ok();
    }
}
