using AutoMapper;
using FinanceApi.Application.Dtos.Funds;
using FinanceApi.Application.Queries.Funds;
using FinanceApi.Controllers.Base;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Queries;

[Route("api/funds")]
public class FundQueryController(IMapper mapper, IMediator mediator)
    : ApiBaseQueryController<Fund?, Guid, FundDto>(mapper, mediator)
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetFundsQuery request)
        => await Handle(request);

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromQuery] GetSingleFundQuery request)
        => await Handle(request);

    [HttpGet("paginated")]
    public async Task<IActionResult> GetPaginated([FromQuery] GetPaginatedFundsQuery request)
        => await Handle(request);

    [HttpGet("latest/{appModuleId}")]
    public async Task<IActionResult> Latest(Guid appModuleId)
        => await Handle(new GetLatestFundQuery(appModuleId));
}