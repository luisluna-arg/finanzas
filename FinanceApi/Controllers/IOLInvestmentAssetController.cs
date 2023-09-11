using AutoMapper;
using FinanceApi.Application.Dtos.IOLInvestmentAssets;
using FinanceApi.Application.Queries.IOLInvestmentAssets;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers;

[Route("api/iol-investment-asset")]
public class IOLInvestmentAssetController : ApiBaseController<IOLInvestmentAsset, Guid, IOLInvestmentAssetDto>
{
    public IOLInvestmentAssetController(IMapper mapper, IMediator mediator)
        : base(mapper, mediator)
    {
    }

    [HttpGet]
    public async Task<IActionResult> GetTypes()
    => await Handle(new GetAllIOLInvestmentAssetsQuery());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTypeById(Guid id)
        => await Handle(new GetIOLInvestmentAssetQuery { Id = id });
}
