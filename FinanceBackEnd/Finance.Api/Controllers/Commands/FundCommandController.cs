using Finance.Api.Controllers.Base;
using Finance.Application.Commands.Funds;
using Finance.Application.Dtos.Funds;
using Finance.Application.Mapping;
using Finance.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/funds")]
public class FundCommandController(IMappingService mapper, IMediator mediator)
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