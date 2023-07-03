using AutoMapper;
using FinanceApi.Application.Dtos;
using FinanceApi.Application.Queries;
using FinanceApi.Domain.Models.Base;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers;

[ApiController]
public abstract class ApiBaseController<TEntity, TDto> : ControllerBase
    where TDto : Dto
    where TEntity : Entity
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

    protected async Task<IActionResult> Handle(GetSingleByIdQuery<TEntity> query)
        => Ok(await MapAndSend(query));

    protected async Task<IActionResult> Handle(GetAllQuery<TEntity> query)
        => Ok(await MapAndSend(query));

    protected async Task<IActionResult> Handle(IRequest command)
    {
        await mediator.Send(command);
        return Ok();
    }

    private async Task<TDto> MapAndSend(IRequest<TEntity> command)
        => mapper.Map<TDto>(await mediator.Send(command));

    private async Task<TDto> MapAndSend(IRequest<TEntity[]> command)
        => mapper.Map<TDto>(await mediator.Send(command));
}
