using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Dtos.Users;
using Finance.Application.Mapping;
using Finance.Application.Queries.Resources;
using Finance.Domain.Models.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/resources")]
public class ResourceQueryController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : ApiBaseQueryController<User?, Guid, UserDto>(mapper, dispatcher)
{
    [HttpGet("fund/{fundId}/owner/{userId}")]
    public async Task<IActionResult> GetFundOwner(Guid userId, Guid fundId)
    {
        var resourcePermission = await Dispatcher.DispatchQueryAsync(new GetFundOwnershipQuery(fundId));
        if (resourcePermission.IsSuccess && resourcePermission.Data.Any())
        {
            return Ok(resourcePermission.Data.First());
        }

        return NotFound();
    }
}
