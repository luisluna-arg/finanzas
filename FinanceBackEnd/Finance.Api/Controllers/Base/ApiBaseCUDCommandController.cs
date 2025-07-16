using CQRSDispatch.Interfaces;
using Finance.Application.Dtos.Base;
using Finance.Application.Mapping;
using Finance.Domain.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Base;

public abstract class ApiBaseCUDCommandController<
    TEntity,
    TId,
    TDto,
    TCreateCommand,
    TUpdateCommand,
    TDeleteCommand
    >(IMappingService mapper, IDispatcher dispatcher)
    : ApiBaseCommandController<TEntity, TId, TDto>(mapper, dispatcher)
    where TDto : Dto<TId>
    where TEntity : IEntity?
    where TCreateCommand : ICommand
    where TUpdateCommand : ICommand
    where TDeleteCommand : ICommand
{
    [HttpPost]
    public async Task<IActionResult> Create(TCreateCommand command)
        => await ExecuteAsync(command);

    [HttpPut]
    public async Task<IActionResult> Update(TUpdateCommand command)
        => await ExecuteAsync(command);

    [HttpDelete]
    public async Task<IActionResult> Delete(TDeleteCommand command)
        => await ExecuteAsync(command);
}
