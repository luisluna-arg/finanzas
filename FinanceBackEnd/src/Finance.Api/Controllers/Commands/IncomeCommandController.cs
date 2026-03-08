using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Api.Controllers.Requests;
using Finance.Application.Auth;
using Finance.Application.Legacy.Dtos.Incomes;
using Finance.Application.Legacy.Mapping;
using Finance.Application.Services;
using Finance.Application.Services.Incomes;
using Finance.Domain.Models.Incomes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/incomes")]
public class IncomeCommandController(
    IMappingService mapper,
    IDispatcher<FinanceDispatchContext> dispatcher,
    IncomeService incomeService)
    : ApiBaseCommandController<Income?, Guid, IncomeDto>(mapper, dispatcher)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateIncomeRequest command)
    {
        var result = await incomeService.Create(command, httpRequest: Request);
        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(MappingService.Map<IncomeDto>(result.Data));
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateIncomeRequest command)
    {
        var result = await incomeService.Update(command, httpRequest: Request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(MappingService.Map<IncomeDto>(result.Data));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteIncomeRequest command)
    {
        var result = await incomeService.Delete(command, httpRequest: Request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{resourceId}/owner/{userId}")]
    public async Task<IActionResult> SetResourcePermissions(SetIncomeOwnerRequest request)
    {
        var result = await incomeService.SetOwner(request.ResourceId, httpRequest: Request);

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
        var result = await incomeService.DeleteOwner(request.ResourceId, httpRequest: Request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok();
    }
}
