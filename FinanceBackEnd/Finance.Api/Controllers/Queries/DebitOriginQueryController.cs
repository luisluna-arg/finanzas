using AutoMapper;
using Finance.Api.Controllers.Base;
using Finance.Application.Dtos.DebitOrigins;
using Finance.Application.Queries.DebitOrigins;
using Finance.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/debit-origins")]
public class DebitOriginQueryController(IMapper mapper, IMediator mediator)
    : ApiBaseQueryController<DebitOrigin?, Guid, DebitOriginDto>(mapper, mediator)
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetAllDebitOriginsQuery request)
        => await Handle(request);

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromQuery] GetDebitOriginQuery request)
        => await Handle(request);
}
