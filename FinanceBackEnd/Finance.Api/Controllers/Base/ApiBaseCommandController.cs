using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Dtos.Base;
using Finance.Application.Mapping;
using Finance.Domain.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Base;

[ApiController]
[Authorize(Policy = "AdminPolicy")]
[Produces("application/json")]
public abstract class ApiBaseCommandController<TEntity, TId, TDto>(IMappingService mapper, IDispatcher dispatcher)
    : ApiBaseController<TId, TDto>(mapper, dispatcher)
    where TDto : Dto<TId>
    where TEntity : IEntity?
{
    protected async override Task<IActionResult> ExecuteAsync(ICommand command)
    {
        await Dispatcher.DispatchCommandAsync(command);
        return Ok();
    }

    protected async Task<IActionResult> Handle404<TCommand>(TCommand command)
        where TCommand : ICommand<DataResult<TEntity>>
    {
        var result = await MapAndSend404<TCommand, TEntity>(command);
        return result == null ? NotFound() : Ok(result);
    }

    protected async Task<IActionResult> Handle404Nullable<TCommand>(TCommand command)
        where TCommand : ICommand<DataResult<TEntity?>>
    {
        var result = await MapAndSend404<TCommand, TEntity?>(command);
        return result == null ? NotFound() : Ok(result);
    }

    private async Task<TDto?> MapAndSend404<TCommand, TResultEntity>(TCommand command)
        where TCommand : ICommand<DataResult<TResultEntity>>
        where TResultEntity : IEntity?
    {
        var dataResult = await Dispatcher.DispatchAsync(command);
        return dataResult.IsSuccess && dataResult.Data != null
            ? MappingService.Map<TDto>(dataResult.Data)
            : null;
    }
}
