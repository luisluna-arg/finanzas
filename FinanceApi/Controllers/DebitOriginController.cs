using AutoMapper;
using FinanceApi.Application.Commands.DebitOrigins;
using FinanceApi.Application.Dtos.DebitOrigins;
using FinanceApi.Application.Queries.DebitOrigins;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers;

[Route("api/debit-origins")]
public class DebitOriginController : ApiBaseController<DebitOrigin?, Guid, DebitOriginDto>
{
    public DebitOriginController(IMapper mapper, IMediator mediator)
        : base(mapper, mediator)
    {
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetAllDebitOriginsQuery request)
        => await Handle(request);

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromQuery] GetDebitOriginQuery request)
        => await Handle(request);

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
