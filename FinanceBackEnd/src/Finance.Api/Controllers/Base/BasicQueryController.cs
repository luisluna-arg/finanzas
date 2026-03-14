using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Dtos.Base;
using Finance.Application.Mapping;
using Finance.Domain.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Base;

public abstract class BasicQueryController<TEntity, TId, TDto, TGetAllQuery, TGetByIdQuery>(
    IMappingService mapper,
    IDispatcher<FinanceDispatchContext> dispatcher)
    : ApiBaseQueryController<TEntity?, TId, TDto>(mapper, dispatcher)
    where TDto : Dto<TId>
    where TEntity : class, IEntity
    where TGetAllQuery : class, IQuery<List<TEntity>>
    where TGetByIdQuery : class, IQuery<TEntity?>
{
    /// <summary>
    /// Gets all entities of this type.
    /// </summary>
    /// <param name="request">The query parameters.</param>
    /// <returns>A collection of entities.</returns>
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] TGetAllQuery request)
        => Ok(await MapAndSendList<TGetAllQuery, TEntity>(request));

    /// <summary>
    /// Gets an entity by its ID.
    /// </summary>
    /// <param name="request">The query with the ID parameter.</param>
    /// <returns>The entity with the specified ID.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromQuery] TGetByIdQuery request)
        => Ok(await MapAndSendSingle<TGetByIdQuery, TEntity>(request));
}
