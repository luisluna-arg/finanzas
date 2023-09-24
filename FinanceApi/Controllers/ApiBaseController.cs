using AutoMapper;
using FinanceApi.Application.Dtos;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain.Models.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers;

[ApiController]
public abstract class ApiBaseController<TEntity, TId, TDto> : ControllerBase
    where TDto : Dto<TId>
    where TEntity : IEntity?
{
    private readonly IMapper mapper;
    private readonly IMediator mediator;

    protected ApiBaseController(IMapper mapper, IMediator mediator)
    {
        this.mapper = mapper;
        this.mediator = mediator;
    }

    protected async Task<IActionResult> Handle(IRequest<TEntity> command)
        => Ok(await MapAndSend(command));

    protected async Task<IActionResult> Handle(GetSingleByIdQuery<TEntity, TId> query)
        => Ok(await MapAndSend(query));

    protected async Task<IActionResult> Handle(GetAllQuery<TEntity> query)
        => Ok(await MapAndSend(query));

    protected async Task<IActionResult> Handle(IRequest command)
    {
        await mediator.Send(command);
        return Ok();
    }

    protected async Task<IActionResult> Handle404(IRequest<TEntity> command)
    {
        var result = await MapAndSend404(command);
        return result == null ? this.NotFound() : Ok(result);
    }

    private async Task<TDto> MapAndSend(IRequest<TEntity> command)
        => mapper.Map<TDto>(await mediator.Send(command));

    private async Task<TDto[]> MapAndSend(IRequest<ICollection<TEntity>> command)
        => (await mediator.Send(command)).Select(entity => mapper.Map<TDto>(entity)).ToArray();

    private async Task<TDto?> MapAndSend404(IRequest<TEntity> command)
    {
        var entity = await mediator.Send(command);
        if (entity != null)
        {
            return mapper.Map<TDto>(entity);
        }

        return null;
    }
}
