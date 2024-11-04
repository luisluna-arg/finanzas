using AutoMapper;
using Finance.Application.Dtos;
using Finance.Domain.Models.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Base;

public abstract class ApiBaseCUDCommandController<
    TEntity,
    TId,
    TDto,
    TCreateCommand,
    TUpdateCommand,
    TDeleteCommand
    >(IMapper mapper, IMediator mediator)
    : ApiBaseCommandController<TEntity, TId, TDto>(mapper, mediator)
    where TDto : Dto<TId>
    where TEntity : IEntity?
    where TCreateCommand : IRequest<TEntity>
    where TUpdateCommand : IRequest<TEntity>
    where TDeleteCommand : IRequest
{
    [HttpPost]
    public async Task<IActionResult> Create(TCreateCommand command)
        => await Handle(command);

    [HttpPut]
    public async Task<IActionResult> Update(TUpdateCommand command)
        => await Handle(command);

    [HttpDelete]
    public async Task<IActionResult> Delete(TDeleteCommand command)
        => await Handle(command);
}
