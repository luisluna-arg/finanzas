using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Api.Controllers.Requests;
using Finance.Api.Controllers.Requests.Identities;
using Finance.Application.Dtos.Users;
using Finance.Application.Mapping;
using Finance.Application.Services;
using Finance.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/identities")]
public class IdentityCommandController(IMappingService mapper, IdentityService identityService, IDispatcher dispatcher)
    : ApiBaseCommandController<Identity?, Guid, UserDto>(mapper, dispatcher)
{
    private IdentityService IdentityService { get => identityService; }

    [HttpPost]
    public async Task<IActionResult> Create(CreateIdentityRequest request)
    {
        var (identity, success, error) = await IdentityService.Create(
            new CreateIdentitySagaRequest(
                request.UserId,
                request.Provider,
                request.SourceId));

        if (!success)
        {
            return BadRequest(new { Error = error });
        }

        return Ok(identity);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateIdentityRequest request)
    {
        var (identity, success, error) = await IdentityService.Update(
            new UpdateIdentitySagaRequest(
                request.IdentityId,
                request.UserId,
                request.Provider,
                request.SourceId));

        if (!success)
        {
            return BadRequest(new { Error = error });
        }

        return Ok(identity);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteIdentityRequest request)
    {
        var (success, error) = await IdentityService.Delete(
            new DeleteIdentitySagaRequest(
                request.IdentityId,
                request.UserId));
        if (!success)
        {
            return BadRequest(new { Error = error });
        }

        return Ok();
    }
}
