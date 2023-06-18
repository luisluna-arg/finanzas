using AutoMapper;
using FinanceApi.Application.Commands.Currencies;
using FinanceApi.Application.Dtos.Currency;
using FinanceApi.Application.Queries.Currencies;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers;

[ApiController]
[Route("api/currency")]
public class CurrencyController : ControllerBase
{
    private readonly IMapper mapper;
    private readonly IMediator mediator;

    public CurrencyController(IMapper mapper, IMediator mediator)
    {
        this.mapper = mapper;
        this.mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
        => Ok(mapper.Map<CurrencyDto[]>(await mediator.Send(new GetAllCurrenciesQuery())));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
        => Ok(mapper.Map<CurrencyDto>(await mediator.Send(new GetCurrencyQuery { Id = id })));

    [HttpPost]
    public async Task<IActionResult> Create(CreateCurrencyCommand command)
        => Ok(mapper.Map<CurrencyDto>(await mediator.Send(command)));

    [HttpPut]
    public async Task<IActionResult> Update(UpdateCurrencyCommand command)
        => Ok(mapper.Map<CurrencyDto>(await mediator.Send(command)));

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteCurrencyCommand command)
    {
        await mediator.Send(command);
        return Ok();
    }
}
