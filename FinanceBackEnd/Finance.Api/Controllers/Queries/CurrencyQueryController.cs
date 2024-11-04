using AutoMapper;
using Finance.Api.Controllers.Base;
using Finance.Application.Dtos.Currencies;
using Finance.Application.Queries.Currencies;
using Finance.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/currencies")]
public class CurrencyQueryController(IMapper mapper, IMediator mediator)
    : ApiBaseQueryController<Currency?, Guid, CurrencyDto>(mapper, mediator)
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetAllCurrenciesQuery request)
        => await Handle(request);

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromQuery] GetCurrencyQuery request)
        => await Handle(request);
}
