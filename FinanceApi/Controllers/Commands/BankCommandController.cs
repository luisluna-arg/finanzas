using AutoMapper;
using FinanceApi.Application.Commands.Banks;
using FinanceApi.Application.Dtos.Banks;
using FinanceApi.Controllers.Base;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Commands;

[Route("api/banks")]
public class BankCommandController(IMapper mapper, IMediator mediator)
    : ApiBaseCommandController<Bank?, Guid, BankDto>(mapper, mediator)
{
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
