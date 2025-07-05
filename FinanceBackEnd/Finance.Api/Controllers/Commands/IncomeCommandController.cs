using Finance.Api.Controllers.Base;
using Finance.Application.Commands.Incomes;
using Finance.Application.Dtos.Incomes;
using Finance.Application.Mapping;
using Finance.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/incomes")]
public class IncomeCommandController(IMappingService mapper, IMediator mediator)
    : ApiBaseCommandController<Income?, Guid, IncomeDto>(mapper, mediator)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateIncomeCommand command)
        => await Handle(command);

    [HttpPut]
    public async Task<IActionResult> Update(UpdateIncomeCommand command)
        => await Handle(command);

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteIncomesCommand request)
        => await Handle(request);
}