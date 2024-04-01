using AutoMapper;
using FinanceApi.Application.Commands.CurrencyConversions;
using FinanceApi.Application.Commands.CurrencyConvertions;
using FinanceApi.Application.Dtos.CurrencyConversions;
using FinanceApi.Controllers.Base;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Commands;

[Route("api/currencies/conversions")]
public class CurrencyConversionCommandController(IMapper mapper, IMediator mediator)
    : ApiBaseCommandController<CurrencyConversion?, Guid, CurrencyConversionDto>(mapper, mediator)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateCurrencyConversionCommand command)
        => await Handle(command);

    [HttpPut]
    public async Task<IActionResult> Update(UpdateCurrencyConversionCommand command)
        => await Handle(command);

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
        => await Handle(new DeleteCurrencyConversionCommand { Id = id });

    [HttpPatch("activate/{id}")]
    public async Task<IActionResult> Activate(Guid id)
        => await Handle(new ActivateCurrencyConversionCommand { Id = id });

    [HttpPatch("deactivate/{id}")]
    public async Task<IActionResult> Deactivate(Guid id)
        => await Handle404(new DeactivateCurrencyConversionCommand { Id = id });
}
