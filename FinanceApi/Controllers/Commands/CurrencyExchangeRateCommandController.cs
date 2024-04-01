using AutoMapper;
using FinanceApi.Application.Commands.CurrencyExchangeRates;
using FinanceApi.Application.Dtos;
using FinanceApi.Controllers.Base;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Commands;

[Route("api/currencies/exchange-rates")]
public class CurrencyExchangeRateController(IMapper mapper, IMediator mediator)
    : ApiBaseCommandController<CurrencyExchangeRate?, Guid, CurrencyExchangeRateDto>(mapper, mediator)
{
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
