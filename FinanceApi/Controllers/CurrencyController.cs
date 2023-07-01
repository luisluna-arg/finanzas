using AutoMapper;
using FinanceApi.Application.Commands.Currencies;
using FinanceApi.Application.Dtos.Currencies;
using FinanceApi.Application.Queries.Currencies;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers;

[Route("api/currency")]
public class CurrencyController : ApiBaseController<Currency, CurrencyDto>
{
    public CurrencyController(IMapper mapper, IMediator mediator)
        : base(mapper, mediator)
    {
    }

    [HttpGet]
    public async Task<IActionResult> Get()
        => await Handle(new GetAllCurrenciesQuery());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
        => await Handle(new GetCurrencyQuery { Id = id });

    [HttpPost]
    public async Task<IActionResult> Create(CreateCurrencyCommand command)
        => await Handle(command);

    [HttpPut]
    public async Task<IActionResult> Update(UpdateCurrencyCommand command)
        => await Handle(command);

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteCurrencyCommand command)
        => await Handle(command);
}
