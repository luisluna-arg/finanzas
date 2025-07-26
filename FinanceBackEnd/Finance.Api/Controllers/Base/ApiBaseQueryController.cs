using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commons;
using Finance.Application.Dtos.Base;
using Finance.Application.Mapping;
using Finance.Application.Queries.Base;
using Finance.Domain.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Base;

public abstract class ApiBaseQueryController<TEntity, TId, TDto>(IMappingService mappingService, IDispatcher<FinanceDispatchContext> dispatcher)
    : ApiBaseController<TId, TDto>(mappingService, dispatcher)
    where TDto : Dto<TId>
    where TEntity : IEntity?
{
    protected async Task<IActionResult> ExecuteAsync(GetAllQuery<TEntity> query)
        => Ok(await MapAndSend(query));

    protected async Task<IActionResult> ExecuteAsync(GetSingleByIdQuery<TEntity, TId> query)
        => Ok(await MapAndSend(query));

    protected async Task<IActionResult> ExecuteAsync(IQuery<PaginatedResult<TEntity>> query)
        => Ok(await MapAndSend(query));

    protected async Task<TDto[]> MapAndSendList<TQuery, TEntityType>(TQuery query)
    where TQuery : IQuery<List<TEntityType>>
    where TEntityType : class
    {
        var dataResult = await Dispatcher.DispatchQueryAsync(query);

        try
        {
            var result = dataResult.Data.Select(entity =>
            {
                try
                {
                    return MappingService.Map<TDto>(entity!);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error mapping entity of type {typeof(TEntityType).Name} to {typeof(TDto).Name}: {ex.Message}");
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

    protected async Task<TDto?> MapAndSendSingle<TQuery, TEntityType>(TQuery query)
        where TQuery : IQuery<TEntityType?>
        where TEntityType : class
    {
        var dataResult = await Dispatcher.DispatchQueryAsync(query);
        try
        {
            return dataResult.Data != null ? MappingService.Map<TDto>(dataResult.Data) : default;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error mapping single entity of type {typeof(TEntityType).Name} to {typeof(TDto).Name}: {ex.Message}");
            Console.WriteLine($"Entity: {System.Text.Json.JsonSerializer.Serialize(dataResult.Data)}");
            throw;
        }
    }

    private async Task<TDto[]> MapAndSend(IQuery<List<TEntity>> query)
    {
        var dataResult = await Dispatcher.DispatchQueryAsync(query);

        try
        {
            var result = dataResult.Data.Select(entity =>
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

    private async Task<TDto> MapAndSend(IQuery<TEntity?> query)
    {
        var dataResult = await Dispatcher.DispatchQueryAsync(query);
        try
        {
            return MappingService.Map<TDto>(dataResult.Data!);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error mapping single entity of type {typeof(TEntity).Name} to {typeof(TDto).Name}: {ex.Message}");
            Console.WriteLine($"Entity: {System.Text.Json.JsonSerializer.Serialize(dataResult.Data)}");
            throw;
        }
    }

    private async Task<PaginatedResult<TDto>> MapAndSend(IQuery<PaginatedResult<TEntity>> query)
    {
        var dataResult = await Dispatcher.DispatchQueryAsync(query);
        try
        {
            return MappingService.Map<PaginatedResult<TDto>>(dataResult.Data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error mapping PaginatedResult of type {typeof(TEntity).Name} to {typeof(TDto).Name}: {ex.Message}");
            throw;
        }
    }
}
