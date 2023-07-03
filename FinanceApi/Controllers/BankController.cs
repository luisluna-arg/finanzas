using AutoMapper;
using FinanceApi.Application.Commands.Banks;
using FinanceApi.Application.Dtos.Banks;
using FinanceApi.Application.Queries.Banks;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers;

[Route("api/banks")]
public class BankController : ApiBaseController<Bank, BankDto>
{
    public BankController(IMapper mapper, IMediator mediator)
        : base(mapper, mediator)
    {
    }

    [HttpGet]
    public async Task<IActionResult> Get()
        => await Handle(new GetAllBanksQuery());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
        => await Handle(new GetBankQuery { Id = id });

    [HttpPost]
    public async Task<IActionResult> Create(CreateBankCommand command)
        => await Handle(command);

    [HttpPut]
    public async Task<IActionResult> Update(UpdateBankCommand command)
        => await Handle(command);
}
