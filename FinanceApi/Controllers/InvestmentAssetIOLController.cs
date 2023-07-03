using AutoMapper;
using FinanceApi.Application.Commands.InvestmentAssetIOLs;
using FinanceApi.Application.Dtos.InvestmentAssetIOLs;
using FinanceApi.Application.Queries.InvestmentAssetIOLs;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers;

[Route("api/investment-asset-iols")]
public class InvestmentAssetIOLController : ApiBaseController<InvestmentAssetIOL, Guid, InvestmentAssetIOLDto>
{
    public InvestmentAssetIOLController(IMapper mapper, IMediator mediator)
        : base(mapper, mediator)
    {
    }

    [HttpGet]
    public async Task<IActionResult> Get()
        => await Handle(new GetAllInvestmentAssetIOLsQuery());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
        => await Handle(new GetInvestmentAssetIOLQuery { Id = id });

    [HttpPost]
    public async Task<IActionResult> Create(CreateInvestmentAssetIOLCommand command)
        => await Handle(command);

    [HttpPut]
    public async Task<IActionResult> Update(UpdateInvestmentAssetIOLCommand command)
        => await Handle(command);
}
