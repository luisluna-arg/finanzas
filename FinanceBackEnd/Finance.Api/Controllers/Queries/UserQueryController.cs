using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Dtos.Users;
using Finance.Application.Mapping;
using Finance.Application.Queries.Users;
using Finance.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/users")]
public class UserQueryController(IMappingService mapper, IDispatcher dispatcher)
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
