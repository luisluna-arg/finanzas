using System.ComponentModel;
using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Api.Controllers.Requests;
using Finance.Application.Auth;
using Finance.Application.Helpers;
using Finance.Application.Legacy.Commands.Debits;
using Finance.Application.Legacy.Dtos.Debits;
using Finance.Application.Legacy.Mapping;
using Finance.Application.Services;
using Finance.Application.Services.Debits;
using Finance.Domain.Enums;
using Finance.Domain.Models.Debits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

public abstract class DebitCommandController(
    IMappingService mapper,
    IDispatcher<FinanceDispatchContext> dispatcher,
    DebitService debitService)
    : ApiBaseCommandController<Debit?, Guid, DebitDto>(mapper, dispatcher)
{
    [HttpPut]
    public async Task<IActionResult> Update(UpdateDebitRequest request)
    {
        var result = await debitService.Update(request, Request);
        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);
        return Ok(MappingService.Map<DebitDto>(result.Data));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteDebitRequest request)
    {
        var result = await debitService.Delete(request, Request);
        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);
        return Ok();
    }

    [HttpPost]
    [Route("activate")]
    public async Task<IActionResult> Activate(ActivateDebitRequest request)
    {
        var result = await debitService.Activate(request, Request);
        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);
        return Ok();
    }

    [HttpPost]
    [Route("deactivate")]
    public async Task<IActionResult> Deactivate(DeactivateDebitRequest request)
    {
        var result = await debitService.Deactivate(request, Request);
        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);
        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{resourceId}/owner/{userId}")]
    public async Task<IActionResult> SetOwner(SetDebitOwnerRequest request)
    {
        var result = await debitService.SetOwner(request.ResourceId, Request);
        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);
        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{resourceId}/owner/{userId}")]
    public async Task<IActionResult> DeleteOwner(DeleteDebitOwnerRequest request)
    {
        var result = await debitService.DeleteOwner(request.ResourceId, Request);
        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);
        return Ok();
    }

    [HttpPost]
    protected async Task<IActionResult> Create(CreateDebitRequest request)
    {
        var result = await debitService.Create(request, Request);
        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);
        return Ok(MappingService.Map<DebitDto>(result.Data));
    }

    [HttpPost]
    [Route("upload")]
    protected async Task<IActionResult> Upload(IFormFile file, string appModuleId, [DefaultValue("Local")] string dateKind, FrequencyEnum frequency)
    {
        await ExecuteAsync(new UploadDebitsFileCommand(file, appModuleId, EnumHelper.Parse<DateTimeKind>(dateKind), frequency));
        return Ok();
    }
}
