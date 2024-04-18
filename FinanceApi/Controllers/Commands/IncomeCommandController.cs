using AutoMapper;
using FinanceApi.Application.Commands.Incomes;
using FinanceApi.Application.Dtos.Incomes;
using FinanceApi.Controllers.Base;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Commands;

[Route("api/incomes")]
public class IncomeCommandController(IMapper mapper, IMediator mediator)
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