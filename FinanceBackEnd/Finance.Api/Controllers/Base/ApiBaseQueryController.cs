using Finance.Application.Commons;
using Finance.Application.Dtos.Base;
using Finance.Application.Mapping;
using Finance.Application.Queries.Base;
using Finance.Domain.Models.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Base;

[ApiController]
public abstract class ApiBaseQueryController<TEntity, TId, TDto>(IMappingService mappingService, IMediator mediator)
    : ApiBaseController<TEntity, TId, TDto>(mappingService, mediator)
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

        try
        {
            var result = current.Select(entity =>
            {
                try
                {
                    return MappingService.Map<TDto>(entity!);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error mapping entity of type {typeof(TEntity).Name} to {typeof(TDto).Name}: {ex.Message}");
                    Console.WriteLine($"Entity: {System.Text.Json.JsonSerializer.Serialize(entity)}");
                    throw;
                }
            }).ToArray();
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in MapAndSend: {ex.Message}");
            throw;
        }
    }

    private async Task<TDto> MapAndSend(IRequest<TEntity> query)
    {
        var entity = await Mediator.Send(query);
        try
        {
            return MappingService.Map<TDto>(entity!);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error mapping single entity of type {typeof(TEntity).Name} to {typeof(TDto).Name}: {ex.Message}");
            Console.WriteLine($"Entity: {System.Text.Json.JsonSerializer.Serialize(entity)}");
            throw;
        }
    }

    private async Task<PaginatedResult<TDto>> MapAndSend(IRequest<PaginatedResult<TEntity>> query)
    {
        var result = await Mediator.Send(query);
        try
        {
            return MappingService.Map<PaginatedResult<TDto>>(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error mapping PaginatedResult of type {typeof(TEntity).Name} to {typeof(TDto).Name}: {ex.Message}");
            throw;
        }
    }
}
