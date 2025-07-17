using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Api.Controllers.Requests;
using Finance.Application.Commands;
using Finance.Application.Dtos.Users;
using Finance.Application.Mapping;
using Finance.Application.Services;
using Finance.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/users")]
public class UserCommandController(IMappingService mapper, UserService userService, IDispatcher dispatcher)
    : ApiBaseCommandController<User?, Guid, UserDto>(mapper, dispatcher)
{
    private UserService UserService { get => userService; }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserRequest request)
    {
        var (user, _) = await UserService.Create(
            new CreateUserSagaRequest(
                request.Email,
                request.FirstName,
                request.LastName,
                request.Roles,
                request.Identities));
        return Ok(user);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateUserRequest request)
    {
        var (user, _) = await UserService.Update(
            new UpdateUserSagaRequest(
                request.Id,
                request.Email,
                request.FirstName,
                request.LastName,
                request.Roles,
                request.Identities));
        return Ok(user);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteUserRequest request)
    {
        await UserService.Delete(new DeleteUserSagaRequest(request.Id));
        return Ok();
    }
}
