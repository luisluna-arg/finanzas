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
    /// <summary>
    /// Gets all entities of this type.
    /// </summary>
    /// <param name="request">The query parameters.</param>
    /// <returns>A collection of entities.</returns>
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] TGetAllQuery request)
        => await Handle(request);

    /// <summary>
    /// Gets an entity by its ID.
    /// </summary>
    /// <param name="request">The query with the ID parameter.</param>
    /// <returns>The entity with the specified ID.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromQuery] TGetByIdQuery request)
        => await Handle(request);
}
