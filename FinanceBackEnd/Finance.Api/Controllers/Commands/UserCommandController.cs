using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Api.Controllers.Requests;
using Finance.Application.Auth;
using Finance.Application.Commands.Users;
using Finance.Application.Dtos.Users;
using Finance.Application.Mapping;
using Finance.Application.Services;
using Finance.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/users")]
public class UserCommandController(IMappingService mapper, UserService userService, IDispatcher<FinanceDispatchContext> dispatcher)
    : ApiBaseCommandController<User?, Guid, UserDto>(mapper, dispatcher)
{
    private UserService UserService { get => userService; }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserRequest request)
    {
        var result = await UserService.Create(
            new CreateUserSagaRequest(
                request.Email,
                request.FirstName,
                request.LastName,
                request.Roles,
                request.Identities));
        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateUserRequest request)
    {
        var result = await UserService.Update(
            new UpdateUserSagaRequest(
                request.Id,
                request.Email,
                request.FirstName,
                request.LastName,
                request.Roles,
                request.Identities));
        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteUserRequest request)
    {
        var result = await UserService.Delete(new DeleteUserSagaRequest(request.Id));
        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok();
    }

    [HttpPut("{userId}/roles")]
    public async Task<IActionResult> SetRoles(Guid userId, UpdateUserRolesCommand command)
    {
        command.UserId = userId;
        await ExecuteAsync(command);
        return Ok();
    }
}
