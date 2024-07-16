using AutoMapper;
using FinanceApi.Application.Dtos.Banks;
using FinanceApi.Application.Queries.Banks;
using FinanceApi.Controllers.Base;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Queries;

[Route("api/banks")]
public class BankQueryController(IMapper mapper, IMediator mediator) : ApiBaseQueryController<Bank?, Guid, BankDto>(mapper, mediator)
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetAllBanksQuery request)
        => await Handle(request);

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromQuery] GetBankQuery request)
        => await Handle(request);
}
