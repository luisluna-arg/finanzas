using AutoMapper;
using Finance.Api.Controllers.Base;
using Finance.Application.Dtos.CurrencyConversions;
using Finance.Application.Queries.CurrencyConvertions;
using Finance.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

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
