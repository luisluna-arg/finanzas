using AutoMapper;
using FinanceApi.Application.Dtos.DebitOrigins;
using FinanceApi.Application.Queries.DebitOrigins;
using FinanceApi.Controllers.Base;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Queries;

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
