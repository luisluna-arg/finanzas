using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Commands.Currencies;
using Finance.Application.Dtos.Currencies;
using Finance.Application.Mapping;
using Finance.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/currencies")]
public class CurrencyCommandController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : ApiBaseCommandController<Currency?, Guid, CurrencyDto>(mapper, dispatcher)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateCurrencyCommand command)
        => await ExecuteAsync(command);

    [HttpPut]
    public async Task<IActionResult> Update(UpdateCurrencyCommand command)
        => await ExecuteAsync(command);

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
        => await ExecuteAsync(new DeleteCurrencyCommand() { Id = id });

    [HttpPatch("activate/{id}")]
    public async Task<IActionResult> Activate(Guid id)
        => await ExecuteAsync(new ActivateCurrencyCommand { Id = id });

    [HttpPatch("deactivate/{id}")]
    public async Task<IActionResult> Deactivate(Guid id)
        => await Handle404Nullable(new DeactivateCurrencyCommand { Id = id });
}
