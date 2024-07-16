using AutoMapper;
using FinanceApi.Application.Commands.Currencies;
using FinanceApi.Application.Dtos.Currencies;
using FinanceApi.Controllers.Base;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Commands;

[Route("api/currencies")]
public class CurrencyCommandController(IMapper mapper, IMediator mediator)
    : ApiBaseCommandController<Currency?, Guid, CurrencyDto>(mapper, mediator)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateCurrencyCommand command)
        => await Handle(command);

    [HttpPut]
    public async Task<IActionResult> Update(UpdateCurrencyCommand command)
        => await Handle(command);

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
        => await Handle(new DeleteCurrencyCommand() { Id = id });

    [HttpPatch("activate/{id}")]
    public async Task<IActionResult> Activate(Guid id)
        => await Handle(new ActivateCurrencyCommand { Id = id });

    [HttpPatch("deactivate/{id}")]
    public async Task<IActionResult> Deactivate(Guid id)
        => await Handle404(new DeactivateCurrencyCommand { Id = id });
}
