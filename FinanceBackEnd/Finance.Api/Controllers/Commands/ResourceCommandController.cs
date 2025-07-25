using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Commands.FundOwners;
using Finance.Application.Dtos.Users;
using Finance.Application.Mapping;
using Finance.Application.Services;
using Finance.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/resources")]
public class ResourceCommandController(IMappingService mapper, FundResourceOwnerService fundResourceOwnerService, IDispatcher dispatcher)
    : ApiBaseCommandController<User?, Guid, UserDto>(mapper, dispatcher)
{
    private FundResourceOwnerService ResourceService { get => fundResourceOwnerService; }

    [HttpPost("fund/{fundId}/owner/{userId}")]
    public async Task<IActionResult> SetFundOwner(Guid fundId, Guid userId)
    {
        var result = await ResourceService.Set(new SetFundOwnerSagaRequest(userId, fundId));
        if (!result.success)
        {
            return BadRequest(result.result);
        }

        return Ok();
    }

    [HttpDelete("fund/{fundId}/owner/{userId}")]
    public async Task<IActionResult> DeleteFundOwner(Guid fundId, Guid userId)
    {
        var result = await ResourceService.Delete(new DeleteFundOwnerSagaRequest(userId, fundId));
        if (!result)
        {
            return BadRequest(result);
        }

        return Ok();
    }
}