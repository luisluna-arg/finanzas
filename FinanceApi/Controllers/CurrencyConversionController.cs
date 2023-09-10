using AutoMapper;
using FinanceApi.Application.Commands.CurrencyConvertions;
using FinanceApi.Application.Dtos.CurrencyConversions;
using FinanceApi.Application.Queries.CurrencyConvertions;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers;

[Route("api/currency-conversions")]
public class CurrencyConversionController : ApiBaseController<CurrencyConversion, Guid, CurrencyConversionDto>
{
    public CurrencyConversionController(IMapper mapper, IMediator mediator)
        : base(mapper, mediator)
    {
    }

    [HttpGet]
    public async Task<IActionResult> Get()
        => await Handle(new GetAllCurrencyConversionsQuery());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
        => await Handle(new GetCurrencyConversionQuery { Id = id });

    [HttpPost]
    public async Task<IActionResult> Create(CreateCurrencyConversionCommand command)
        => await Handle(command);

    [HttpPut]
    public async Task<IActionResult> Update(UpdateCurrencyConversionCommand command)
        => await Handle(command);

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
        => await Handle(new DeleteCurrencyConversionCommand { Id = id });
}
