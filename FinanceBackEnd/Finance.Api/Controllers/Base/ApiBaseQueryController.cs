using AutoMapper;
using Finance.Application.Commons;
using Finance.Application.Dtos;
using Finance.Application.Queries.Base;
using Finance.Domain.Models.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Base;

[ApiController]
public abstract class ApiBaseQueryController<TEntity, TId, TDto>(IMapper mapper, IMediator mediator)
    : ApiBaseController<TEntity, TId, TDto>(mapper, mediator)
    where TDto : Dto<TId>
    where TEntity : IEntity?
{
    protected async Task<IActionResult> Handle(GetAllQuery<TEntity> query)
        => Ok(await MapAndSend(query));

    protected async Task<IActionResult> Handle(GetSingleByIdQuery<TEntity, TId> query)
        => Ok(await MapAndSend(query));

    protected async Task<IActionResult> Handle(IRequest<PaginatedResult<TEntity>> query)
        => Ok(await MapAndSend(query));

    private async Task<TDto[]> MapAndSend(IRequest<ICollection<TEntity>> query)
    {
        var current = await Mediator.Send(query);
        var result = current.Select(entity => Mapper.Map<TDto>(entity)).ToArray();
        return result;
    }

    private async Task<TDto> MapAndSend(IRequest<TEntity> query)
        => Mapper.Map<TDto>(await Mediator.Send(query));

    private async Task<PaginatedResult<TDto>> MapAndSend(IRequest<PaginatedResult<TEntity>> query)
        => Mapper.Map<PaginatedResult<TDto>>(await Mediator.Send(query));
}
