using Finance.Api.Controllers.Base;
using Finance.Application.Dtos.IOLInvestmentAssets;
using Finance.Application.Mapping;
using Finance.Application.Queries.IOLInvestmentAssets;
using Finance.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/iol-investment-asset")]
public class IOLInvestmentAssetQueryController(IMappingService mapper, IMediator mediator)
    : ApiBaseQueryController<IOLInvestmentAsset?, Guid, IOLInvestmentAssetDto>(mapper, mediator)
{
    [HttpGet]
    public async Task<IActionResult> GetTypes()
    => await Handle(new GetAllIOLInvestmentAssetsQuery());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTypeById(Guid id)
        => await Handle(new GetIOLInvestmentAssetQuery { Id = id });
}
