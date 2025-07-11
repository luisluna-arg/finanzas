using Finance.Application.Dtos.Base;
using Finance.Application.Mapping;
using Finance.Domain.Models.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Base;

[ApiController]
[Authorize(Policy = "AdminPolicy")]
[Produces("application/json")]
public abstract class ApiBaseCommandController<TEntity, TId, TDto>(IMappingService mapper, IMediator mediator)
    : ApiBaseController<TEntity, TId, TDto>(mapper, mediator)
    where TDto : Dto<TId>
    where TEntity : IEntity?
{
    protected async Task<IActionResult> Handle(IRequest command)
    {
        await Mediator.Send(command);
        return Ok();
    }

    protected async Task<IActionResult> Handle404(IRequest<TEntity> command)
    {
        var result = await MapAndSend404(command);
        return result == null ? this.NotFound() : Ok(result);
    }

    private async Task<TDto?> MapAndSend404(IRequest<TEntity> command)
    {
        var entity = await Mediator.Send(command);
        return entity != null ? MappingService.Map<TDto>(entity) : null;
    }
}
