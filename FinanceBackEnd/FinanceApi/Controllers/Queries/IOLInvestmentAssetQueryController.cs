using AutoMapper;
using FinanceApi.Application.Dtos.IOLInvestmentAssets;
using FinanceApi.Application.Queries.IOLInvestmentAssets;
using FinanceApi.Controllers.Base;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Queries;

[Route("api/iol-investment-asset")]
public class IOLInvestmentAssetQueryController(IMapper mapper, IMediator mediator)
    : ApiBaseQueryController<IOLInvestmentAsset?, Guid, IOLInvestmentAssetDto>(mapper, mediator)
{
    [HttpGet]
    public async Task<IActionResult> GetTypes()
    => await Handle(new GetAllIOLInvestmentAssetsQuery());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTypeById(Guid id)
        => await Handle(new GetIOLInvestmentAssetQuery { Id = id });
}
