using AutoMapper;
using FinanceApi.Application.Dtos.CurrencyConversions;
using FinanceApi.Application.Queries.CurrencyConvertions;
using FinanceApi.Controllers.Base;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Queries;

[Route("api/currencies/conversions")]
public class CurrencyConversionQueryController(IMapper mapper, IMediator mediator)
    : ApiBaseQueryController<CurrencyConversion?, Guid, CurrencyConversionDto>(mapper, mediator)
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetAllCurrencyConversionsQuery request)
        => await Handle(request);

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromQuery] GetCurrencyConversionQuery request)
        => await Handle(request);
}
