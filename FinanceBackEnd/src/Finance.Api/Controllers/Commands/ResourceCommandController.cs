using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Dtos.Users;
using Finance.Application.Mapping;
using Finance.Application.Services;
using Finance.Domain.Models.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/resources")]
public class ResourceCommandController(IMappingService mapper, FundService fundService, IDispatcher<FinanceDispatchContext> dispatcher)
    : ApiBaseCommandController<User?, Guid, UserDto>(mapper, dispatcher)
{
    [HttpPost("fund/{fundId}/owner/{userId}")]
    public async Task<IActionResult> SetFundOwner(Guid fundId, Guid userId)
    {
        var result = await fundService.SetOwner(fundId);
        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);
    }

    [HttpDelete("fund/{fundId}/owner/{userId}")]
    public async Task<IActionResult> DeleteFundOwner(Guid fundId, Guid userId)
    {
        var result = await fundService.DeleteOwner(fundId);
        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok();
    }
}
