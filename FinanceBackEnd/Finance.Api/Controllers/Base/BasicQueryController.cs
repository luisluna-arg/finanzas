using Finance.Application.Dtos.Base;
using Finance.Application.Mapping;
using Finance.Application.Queries.Base;
using Finance.Domain.Models.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Base;

public abstract class BasicQueryController<TEntity, TId, TDto, TGetAllQuery, TGetByIdQuery>(
    IMappingService mapper,
    IMediator mediator)
    : ApiBaseQueryController<TEntity, TId, TDto>(mapper, mediator)
    where TDto : Dto<TId>
    where TEntity : IEntity?
    where TGetAllQuery : GetAllQuery<TEntity>
    where TGetByIdQuery : GetSingleByIdQuery<TEntity, TId>
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] TGetAllQuery request)
        => await Handle(request);

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromQuery] TGetByIdQuery request)
        => await Handle(request);
}
