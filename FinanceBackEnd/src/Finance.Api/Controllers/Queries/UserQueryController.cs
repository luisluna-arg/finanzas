using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Dtos.Users;
using Finance.Application.Mapping;
using Finance.Application.Queries.Users;
using Finance.Domain.Models.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

// TODO: This should be under Admin Policy protection
[Route("api/users")]
public class UserQueryController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : BasicQueryController<User, Guid, UserDto, GetAllUsersQuery, GetUserByIdQuery>(mapper, dispatcher)
{
    [HttpGet("by-source/{id}")]
    public async Task<IActionResult> GetBySourceId(string id)
    {
        var existingUserResult = await Dispatcher.DispatchQueryAsync(new GetUserBySourceIdsQuery([id]));
        if (existingUserResult.IsSuccess)
        {
            return Ok(MappingService.Map<UserDto>(existingUserResult.Data!));
        }

        return NotFound();
    }
}
