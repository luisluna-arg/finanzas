using AutoMapper;
using FinanceApi.Application.Dtos.InvestmentAssetIOLTypes;
using FinanceApi.Application.Queries.InvestmentAssetIOLs;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers;

[Route("api/investment-asset-iol-types")]
public class InvestmentAssetIOLTypeController : ApiBaseController<InvestmentAssetIOLType, ushort, InvestmentAssetIOLTypeDto>
{
    public InvestmentAssetIOLTypeController(IMapper mapper, IMediator mediator)
        : base(mapper, mediator)
    {
    }

    [HttpGet]
    public async Task<IActionResult> GetTypes()
    => await Handle(new GetAllInvestmentAssetIOLTypesQuery());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTypeById(ushort id)
        => await Handle(new GetInvestmentAssetIOLTypeQuery { Id = id });
}
