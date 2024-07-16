using AutoMapper;
using FinanceApi.Application.Dtos.Currencies;
using FinanceApi.Application.Queries.Currencies;
using FinanceApi.Controllers.Base;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Queries;

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
