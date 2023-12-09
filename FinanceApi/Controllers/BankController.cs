using AutoMapper;
using FinanceApi.Application.Commands.Banks;
using FinanceApi.Application.Dtos.Banks;
using FinanceApi.Application.Queries.Banks;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers;

[Route("api/banks")]
public class BankController : ApiBaseController<Bank?, Guid, BankDto>
{
    public BankController(IMapper mapper, IMediator mediator)
        : base(mapper, mediator)
    {
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetAllBanksQuery request)
        => await Handle(request);

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromQuery] GetBankQuery request)
        => await Handle(request);

    [HttpPost]
    public async Task<IActionResult> Create(CreateBankCommand command)
        => await Handle(command);

    [HttpPut]
    public async Task<IActionResult> Update(UpdateBankCommand command)
        => await Handle(command);

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteBankCommand request)
        => await Handle(request);
}
