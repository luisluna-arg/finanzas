using AutoMapper;
using FinanceApi.Application.Commands.DebitOrigins;
using FinanceApi.Application.Dtos.DebitOrigins;
using FinanceApi.Controllers.Base;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Commands;

[Route("api/debit-origins")]
public class DebitOriginCommandController(IMapper mapper, IMediator mediator)
    : ApiBaseCommandController<DebitOrigin?, Guid, DebitOriginDto>(mapper, mediator)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateDebitOriginCommand command)
        => await Handle(command);

    [HttpPut]
    public async Task<IActionResult> Update(UpdateDebitOriginCommand command)
        => await Handle(command);

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteDebitOriginCommand request)
        => await Handle(request);
}
