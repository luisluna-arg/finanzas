using AutoMapper;
using Finance.Api.Controllers.Base;
using Finance.Application.Commands.Banks;
using Finance.Application.Dtos.Banks;
using Finance.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

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
