using AutoMapper;
using FinanceApi.Application.Commands.Funds;
using FinanceApi.Application.Dtos.Funds;
using FinanceApi.Controllers.Base;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Commands;

[Route("api/funds")]
public class FundCommandController(IMapper mapper, IMediator mediator)
    : ApiBaseCommandController<Fund?, Guid, FundDto>(mapper, mediator)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateFundCommand command)
        => await Handle(command);

    [HttpPut]
    public async Task<IActionResult> Update(UpdateFundCommand command)
        => await Handle(command);

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteFundsCommand request)
        => await Handle(request);
}