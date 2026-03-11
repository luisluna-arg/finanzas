using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Api.Controllers.Requests;
using Finance.Application.Auth;
using Finance.Application.Legacy.Dtos;
using Finance.Application.Legacy.Mapping;
using Finance.Application.Services;
using Finance.Application.Services.CurrencyExchangeRates;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Currencies;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/currencies/exchange-rates")]
public class CurrencyExchangeRateController(
    IMappingService mapper,
    IDispatcher<FinanceDispatchContext> dispatcher,
    CurrencyExchangeRateService currencyExchangeRateService)
    : CommandController<
        CurrencyExchangeRate,
        CurrencyExchangeRatePermissions,
        CreateCurrencyExchangeRateRequest,
        UpdateCurrencyExchangeRateRequest,
        DeleteCurrencyExchangeRateRequest,
        SetCurrencyExchangeRateOwnerRequest,
        DeleteCurrencyExchangeRateOwnerRequest,
        Guid,
        CurrencyExchangeRateDto,
        CurrencyExchangeRateService>(mapper, dispatcher, currencyExchangeRateService)
{
}