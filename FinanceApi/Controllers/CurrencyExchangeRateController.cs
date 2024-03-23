using AutoMapper;
using FinanceApi.Application.Commands.CurrencyExchangeRates;
using FinanceApi.Application.Dtos;
using FinanceApi.Application.Queries.CurrencyExchangeRates;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers;

[Route("api/currencies/exchange-rates")]
public class CurrencyExchangeRateController : ApiBaseController<CurrencyExchangeRate?, Guid, CurrencyExchangeRateDto>
{
    public CurrencyExchangeRateController(IMapper mapper, IMediator mediator)
        : base(mapper, mediator)
    {
    }

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

    [HttpPost]
    public async Task<IActionResult> Create(CreateCurrencyExchangeRateCommand command)
        => await Handle(command);

    [HttpPut]
    public async Task<IActionResult> Update(UpdateCurrencyExchangeRateCommand command)
        => await Handle(command);

    [HttpDelete]
    public async Task<IActionResult> Delete(Guid[] ids)
        => await Handle(new DeleteCurrencyExchangeRateCommand() { Ids = ids });

    [HttpPatch("activate/{id}")]
    public async Task<IActionResult> Activate(Guid id)
        => await Handle(new ActivateCurrencyExchangeRateCommand { Id = id });

    [HttpPatch("deactivate/{id}")]
    public async Task<IActionResult> Deactivate(Guid id)
        => await Handle404(new DeactivateCurrencyExchangeRateCommand { Id = id });
}
