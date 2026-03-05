using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Legacy.Commands.CurrencyConversions;
using Finance.Application.Legacy.Commands.CurrencyConvertions;
using Finance.Application.Legacy.Dtos.CurrencyConversions;
using Finance.Application.Legacy.Mapping;
using Finance.Domain.Models.Currencies;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/currencies/conversions")]
public class CurrencyConversionCommandController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : ApiBaseCommandController<CurrencyConversion?, Guid, CurrencyConversionDto>(mapper, dispatcher)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateCurrencyConversionCommand command)
        => await ExecuteAsync(command);

    [HttpPut]
    public async Task<IActionResult> Update(UpdateCurrencyConversionCommand command)
        => await ExecuteAsync(command);

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
        => await ExecuteAsync(new DeleteCurrencyConversionCommand { Id = id });

    [HttpPatch("activate/{id}")]
    public async Task<IActionResult> Activate(Guid id)
        => await ExecuteAsync(new ActivateCurrencyConversionCommand { Id = id });

    [HttpPatch("deactivate/{id}")]
    public async Task<IActionResult> Deactivate(Guid id)
        => await Handle404Nullable(new DeactivateCurrencyConversionCommand { Id = id });
}
