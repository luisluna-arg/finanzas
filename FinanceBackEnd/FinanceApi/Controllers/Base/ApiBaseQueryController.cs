using AutoMapper;
using FinanceApi.Application.Dtos;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Commons;
using FinanceApi.Domain.Models.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Base;

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
        => (await Mediator.Send(query)).Select(entity => Mapper.Map<TDto>(entity)).ToArray();

    private async Task<TDto> MapAndSend(IRequest<TEntity> query)
        => Mapper.Map<TDto>(await Mediator.Send(query));

    private async Task<PaginatedResult<TDto>> MapAndSend(IRequest<PaginatedResult<TEntity>> query)
        => Mapper.Map<PaginatedResult<TDto>>(await Mediator.Send(query));
}
