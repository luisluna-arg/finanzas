using AutoMapper;
using FinanceApi.Application.Dtos.IOLInvestmentAssetTypes;
using FinanceApi.Application.Queries.IOLInvestmentAssetTypes;
using FinanceApi.Controllers.Base;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Queries;

[Route("api/iol-investment-asset-type")]
public class IOLInvestmentAssetTypeQueryController(IMapper mapper, IMediator mediator)
    : ApiBaseQueryController<IOLInvestmentAssetType?, ushort, IOLInvestmentAssetTypeDto>(mapper, mediator)
{
    [HttpGet]
    public async Task<IActionResult> GetTypes()
    => await Handle(new GetAllIOLInvestmentAssetTypesQuery());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTypeById(ushort id)
        => await Handle(new GetIOLInvestmentAssetTypeQuery { Id = id });
}
