using AutoMapper;
using FinanceApi.Application.Dtos;
using FinanceApi.Domain.Models.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Base;

[ApiController]
public abstract class ApiBaseController<TEntity, TId, TDto>(
    IMapper mapper,
    IMediator mediator)
    : ControllerBase
    where TDto : Dto<TId>
    where TEntity : IEntity?
{
    protected IMapper Mapper { get => mapper; }

    protected IMediator Mediator { get => mediator; }

    protected async Task<IActionResult> Handle(IRequest<TEntity> command)
        => Ok(await MapAndSend(command));

    private async Task<TDto> MapAndSend(IRequest<TEntity> command)
        => Mapper.Map<TDto>(await Mediator.Send(command));
}
