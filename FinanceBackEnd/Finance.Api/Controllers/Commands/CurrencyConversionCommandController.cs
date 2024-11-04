using AutoMapper;
using Finance.Api.Controllers.Base;
using Finance.Application.Commands.CurrencyConversions;
using Finance.Application.Commands.CurrencyConvertions;
using Finance.Application.Dtos.CurrencyConversions;
using Finance.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

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
