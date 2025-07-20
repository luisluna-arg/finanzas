using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Dtos.Users;
using Finance.Application.Mapping;
using Finance.Application.Queries.Resources;
using Finance.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/resources")]
public class ResourceQueryController(IMappingService mapper, IDispatcher dispatcher)
    : ApiBaseQueryController<User?, Guid, UserDto>(mapper, dispatcher)
{
    [HttpGet("fund/{fundId}/owner/{userId}")]
    public async Task<IActionResult> GetFundOwner(Guid userId, Guid fundId)
    {
        var resourceOwnership = await Dispatcher.DispatchQueryAsync(new GetResourceOwnershipQuery(userId, fundId));
        if (resourceOwnership.IsSuccess && resourceOwnership.Data.Any())
        {
            return Ok(resourceOwnership.Data.First());
        }

        return NotFound();
    }
}