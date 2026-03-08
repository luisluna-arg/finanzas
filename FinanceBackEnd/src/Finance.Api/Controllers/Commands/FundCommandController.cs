using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Api.Controllers.Requests;
using Finance.Application.Auth;
using Finance.Application.Legacy.Dtos.Funds;
using Finance.Application.Legacy.Mapping;
using Finance.Application.Services;
using Finance.Application.Services.Funds;
using Finance.Domain.Models.Funds;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/funds")]
public class FundCommandController(
    IMappingService mapper,
    IDispatcher<FinanceDispatchContext> dispatcher,
    FundService fundService)
    : ApiBaseCommandController<Fund?, Guid, FundDto>(mapper, dispatcher)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateFundRequest command)
    {
        var result = await fundService.Create(command, httpRequest: Request);
        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(MappingService.Map<FundDto>(result.Data));
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateFundRequest command)
    {
        var result = await fundService.Update(command, httpRequest: Request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(MappingService.Map<FundDto>(result.Data));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteFundRequest command)
    {
        var result = await fundService.Delete(command, httpRequest: Request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{resourceId}/owner/{userId}")]
    public async Task<IActionResult> SetResourcePermissions(SetFundOwnerRequest request)
    {
        var result = await fundService.SetOwner(request.ResourceId, httpRequest: Request);

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
        var result = await fundService.DeleteOwner(request.ResourceId, httpRequest: Request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok();
    }

    [HttpPost("activate")]
    public async Task<IActionResult> Activate(ActivateFundRequest command)
    {
        var result = await fundService.Activate(command, httpRequest: Request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok();
    }

    [HttpPost("deactivate")]
    public async Task<IActionResult> Deactivate(DeactivateFundRequest command)
    {
        var result = await fundService.Deactivate(command, httpRequest: Request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok();
    }
}
