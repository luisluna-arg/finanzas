using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Requests.Base;
using Finance.Application.Auth;
using Finance.Application.Dtos.Base;
using Finance.Application.Mapping;
using Finance.Application.Services;
using Finance.Domain.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Base;

[ApiController]
[Authorize(Policy = "AdminOrOwnerPolicy")]
[Produces("application/json")]
public abstract class CommandController<
    TEntity,
    TEntityPermissions,
    TCreateRequest,
    TUpdateRequest,
    TDeleteRequest,
    TSetOwnerRequest,
    TDeleteOwnerRequest,
    TId,
    TDto,
    TService>(
        IMappingService mapper,
        IDispatcher<FinanceDispatchContext> dispatcher,
        TService crudService)
        : ApiBaseController<TId, TDto>(mapper, dispatcher)
    where TDto : Dto<TId>
    where TId : struct
    where TEntity : IEntity?
    where TEntityPermissions : class
    where TCreateRequest : class
    where TUpdateRequest : class
    where TDeleteRequest : class
    where TSetOwnerRequest : BaseResourceOwnerRequest<TId>
    where TDeleteOwnerRequest : BaseResourceOwnerRequest<TId>
    where TService : ICRUDService<
        TEntity,
        TId,
        TEntityPermissions,
        TCreateRequest,
        TUpdateRequest,
        TDeleteRequest>
{
    protected TService CrudService
    { get => crudService; }

    [HttpPost]
    public async Task<IActionResult> Create(TCreateRequest command)
    {
        var result = await CrudService.Create(command, httpRequest: Request);
        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(MappingService.Map<TDto>(result.Data!));
    }

    [HttpPut]
    public async Task<IActionResult> Update(TUpdateRequest command)
    {
        var result = await CrudService.Update(command, httpRequest: Request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(MappingService.Map<TDto>(result.Data!));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(TDeleteRequest command)
    {
        var result = await CrudService.Delete(command, httpRequest: Request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{resourceId}/owner/{userId}")]
    public async Task<IActionResult> SetResourcePermissions(TSetOwnerRequest request)
    {
        var result = await CrudService.SetOwner(request.ResourceId, httpRequest: Request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(MappingService.Map<TDto>(result.Data));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{resourceId}/owner/{userId}")]
    public async Task<IActionResult> DeleteResourcePermissions(TDeleteOwnerRequest request)
    {
        var result = await CrudService.DeleteOwner(request.ResourceId, httpRequest: Request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok();
    }

    [HttpPatch("activate/{id}")]
    public async Task<IActionResult> Activate(TId id)
    {
        var result = await CrudService.Activate([id], httpRequest: Request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok();
    }

    [HttpPatch("deactivate/{id}")]
    public async Task<IActionResult> Deactivate(TId id)
    {
        var result = await CrudService.Deactivate([id], httpRequest: Request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok();
    }
}
