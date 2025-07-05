using Finance.Api.Controllers.Base;
using Finance.Application.Commands.DebitOrigins;
using Finance.Application.Dtos.DebitOrigins;
using Finance.Application.Mapping;
using Finance.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/debit-origins")]
public class DebitOriginCommandController(IMappingService mapper, IMediator mediator)
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
