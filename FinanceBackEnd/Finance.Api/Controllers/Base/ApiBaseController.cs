using CQRSDispatch.Interfaces;
using Finance.Application.Dtos.Base;
using Finance.Application.Mapping;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Base;

public abstract class ApiBaseController<TId, TDto>(
    IMappingService mappingService,
    IDispatcher dispatcher)
    : SecuredApiController
    where TDto : Dto<TId>
{
    protected IMappingService MappingService { get => mappingService; }

    protected IDispatcher Dispatcher { get => dispatcher; }

    protected virtual async Task<IActionResult> ExecuteAsync(ICommand command)
        => Ok(await MapAndSend(command));

    protected virtual async Task<TDto?> MapAndSend(ICommand command)
    {
        var entity = await Dispatcher.DispatchCommandAsync(command);
        if (entity is null) return default;
        return MappingService.Map<TDto>(entity);
    }
}
