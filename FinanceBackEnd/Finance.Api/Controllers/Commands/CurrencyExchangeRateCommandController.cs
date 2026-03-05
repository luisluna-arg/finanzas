using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Legacy.Commands.CurrencyExchangeRates;
using Finance.Application.Legacy.Dtos;
using Finance.Application.Legacy.Mapping;
using Finance.Application.Legacy.Services;
using Finance.Application.Legacy.Services.Requests.CurrencyExchangeRates;
using Finance.Domain.Models.Currencies;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/currencies/exchange-rates")]
public class CurrencyExchangeRateController(
    IMappingService mapper,
    IDispatcher<FinanceDispatchContext> dispatcher,
    CurrencyExchangeRateService currencyExchangeRateService)
    : ApiBaseCommandController<CurrencyExchangeRate?, Guid, CurrencyExchangeRateDto>(mapper, dispatcher)
{
    private CurrencyExchangeRateService CurrencyExchangeRateService { get => currencyExchangeRateService; }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCurrencyExchangeRateSagaRequest command)
    {
        var result = await CurrencyExchangeRateService.Create(command, httpRequest: HttpContext?.Request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateCurrencyExchangeRateCommand command)
        => await ExecuteAsync(command);

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteCurrencyExchangeRateCommand command)
        => await ExecuteAsync(command);

    [HttpPatch("activate/{id}")]
    public async Task<IActionResult> Activate(Guid id)
        => await ExecuteAsync(new ActivateCurrencyExchangeRateCommand { Id = id });

    [HttpPatch("deactivate/{id}")]
    public async Task<IActionResult> Deactivate(Guid id)
        => await Handle404Nullable(new DeactivateCurrencyExchangeRateCommand { Id = id });
}
