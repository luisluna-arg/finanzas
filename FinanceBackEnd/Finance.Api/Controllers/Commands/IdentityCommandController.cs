using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Api.Controllers.Requests;
using Finance.Api.Controllers.Requests.Identities;
using Finance.Application.Auth;
using Finance.Application.Dtos.Users;
using Finance.Application.Mapping;
using Finance.Application.Services;
using Finance.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/identities")]
public class IdentityCommandController(IMappingService mapper, IdentityService identityService, IDispatcher<FinanceDispatchContext> dispatcher)
    : ApiBaseCommandController<Identity?, Guid, UserDto>(mapper, dispatcher)
{
    private IdentityService IdentityService { get => identityService; }

    [HttpPost]
    public async Task<IActionResult> Create(CreateIdentityRequest request)
    {
        var result = await IdentityService.Create(
            new CreateIdentitySagaRequest(
                request.UserId,
                request.Provider,
                request.SourceId));

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateIdentityRequest request)
    {
        var result = await IdentityService.Update(
            new UpdateIdentitySagaRequest(
                request.IdentityId,
                request.UserId,
                request.Provider,
                request.SourceId));

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteIdentityRequest request)
    {
        var result = await IdentityService.Delete(
            new DeleteIdentitySagaRequest(
                request.IdentityId,
                request.UserId));
        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok();
    }
}
