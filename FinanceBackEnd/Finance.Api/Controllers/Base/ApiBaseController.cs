using Finance.Application.Dtos.Base;
using Finance.Application.Mapping;
using Finance.Domain.Models.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Base;

[ApiController]
public abstract class ApiBaseController<TEntity, TId, TDto>(
    IMappingService mappingService,
    IMediator mediator)
    : ControllerBase
    where TDto : Dto<TId>
    where TEntity : IEntity?
{
    protected IMappingService MappingService { get => mappingService; }

    protected IMediator Mediator { get => mediator; }

    protected async Task<IActionResult> Handle(IRequest<TEntity> command)
        => Ok(await MapAndSend(command));

    private async Task<TDto?> MapAndSend(IRequest<TEntity> command)
    {
        var entity = await Mediator.Send(command);
        if (entity is null) return default;
        return MappingService.Map<TDto>(entity);
    }
}
