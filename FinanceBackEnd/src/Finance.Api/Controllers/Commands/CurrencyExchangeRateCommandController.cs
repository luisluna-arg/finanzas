using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Api.Controllers.Requests;
using Finance.Application.Auth;
using Finance.Application.Commands.CurrencyExchangeRates;
using Finance.Application.Legacy.Dtos;
using Finance.Application.Legacy.Mapping;
using Finance.Application.Services;
using Finance.Application.Services.CurrencyExchangeRates;
using Finance.Domain.Models.Currencies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/currencies/exchange-rates")]
public class CurrencyExchangeRateController(
    IMappingService mapper,
    IDispatcher<FinanceDispatchContext> dispatcher,
    CurrencyExchangeRateService currencyExchangeRateService)
    : ApiBaseCommandController<CurrencyExchangeRate?, Guid, CurrencyExchangeRateDto>(mapper, dispatcher)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateCurrencyExchangeRateRequest command)
    {
        var result = await currencyExchangeRateService.Create(command, httpRequest: Request);
        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateCurrencyExchangeRateRequest command)
    {
        var result = await currencyExchangeRateService.Update(command, httpRequest: Request);
        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteCurrencyExchangeRateRequest command)
    {
        var result = await currencyExchangeRateService.Delete(command, httpRequest: Request);
        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok();
    }

    [HttpPatch("activate/{id}")]
    public async Task<IActionResult> Activate(Guid id)
        => await Handle404Nullable(new ActivateCurrencyExchangeRateCommand { Id = id });

    [HttpPatch("deactivate/{id}")]
    public async Task<IActionResult> Deactivate(Guid id)
        => await Handle404Nullable(new DeactivateCurrencyExchangeRateCommand { Id = id });

    [Authorize(Roles = "Admin")]
    [HttpPost("{resourceId}/owner/{userId}")]
    public async Task<IActionResult> SetResourcePermissions(SetCurrencyExchangeRateOwnerRequest request)
    {
        var result = await currencyExchangeRateService.SetOwner(request.ResourceId, httpRequest: Request);
        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Data);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{resourceId}/owner/{userId}")]
    public async Task<IActionResult> DeleteResourcePermissions(DeleteCurrencyExchangeRateOwnerRequest request)
    {
        var result = await currencyExchangeRateService.DeleteOwner(request.ResourceId, httpRequest: Request);
        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok();
    }
}