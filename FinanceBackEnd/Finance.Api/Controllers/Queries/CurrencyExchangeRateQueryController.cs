using AutoMapper;
using Finance.Api.Controllers.Base;
using Finance.Application.Dtos;
using Finance.Application.Queries.CurrencyExchangeRates;
using Finance.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/currencies/exchange-rates")]
public class CurrencyExchangeRateQueryController(IMapper mapper, IMediator mediator)
    : ApiBaseQueryController<CurrencyExchangeRate?, Guid, CurrencyExchangeRateDto>(mapper, mediator)
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetAllCurrencyExchangeRatesQuery request)
        => await Handle(request);

    [HttpGet("paginated")]
    public async Task<IActionResult> GetPaginated([FromQuery] GetPaginatedCurrencyExchangeRatesQuery request)
        => await Handle(request);

    [HttpGet]
    [Route("latest")]
    public async Task<IActionResult> Latest([FromQuery] GetLatestCurrencyExchangeRatesQuery request)
        => await Handle(request);

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromQuery] GetCurrencyExchangeRateQuery request)
        => await Handle(request);

    [HttpGet("{QuoteCurrencyShortName}/latest")]
    public async Task<IActionResult> LatestByShortName([FromRoute] GetLatestCurrencyExchangeRateByShortNameQuery request)
        => await Handle(request);
}
