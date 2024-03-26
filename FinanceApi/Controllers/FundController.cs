using AutoMapper;
using FinanceApi.Application.Commands.Funds;
using FinanceApi.Application.Dtos.Funds;
using FinanceApi.Application.Queries.Funds;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers;

[Route("api/funds")]
public class FundController : ApiBaseController<Fund?, Guid, FundDto>
{
    public FundController(IMapper mapper, IMediator mediator)
        : base(mapper, mediator)
    {
    }

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