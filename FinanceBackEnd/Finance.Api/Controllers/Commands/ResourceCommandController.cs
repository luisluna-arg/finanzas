using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Commands.Funds.Owners;
using Finance.Application.Dtos.Users;
using Finance.Application.Mapping;
using Finance.Application.Services;
using Finance.Domain.Models.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/resources")]
public class ResourceCommandController(IMappingService mapper, FundOwnerService fundResourceOwnerService, IDispatcher<FinanceDispatchContext> dispatcher)
    : ApiBaseCommandController<User?, Guid, UserDto>(mapper, dispatcher)
{
    private FundOwnerService ResourceService { get => fundResourceOwnerService; }

    [HttpPost("fund/{fundId}/owner/{userId}")]
    public async Task<IActionResult> SetFundOwner(Guid fundId, Guid userId)
    {
        var result = await ResourceService.Set(new SetFundOwnerSagaRequest(fundId));
        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);
    }

    [HttpDelete("fund/{fundId}/owner/{userId}")]
    public async Task<IActionResult> DeleteFundOwner(Guid fundId, Guid userId)
    {
        var result = await ResourceService.Delete(new DeleteFundOwnerSagaRequest(fundId));
        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok();
    }
}
