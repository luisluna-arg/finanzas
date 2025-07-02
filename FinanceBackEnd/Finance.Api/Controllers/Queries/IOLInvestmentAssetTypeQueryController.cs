using AutoMapper;
using Finance.Api.Controllers.Base;
using Finance.Application.Dtos.IOLInvestmentAssetTypes;
using Finance.Application.Queries.IOLInvestmentAssetTypes;
using Finance.Domain.Enums;
using Finance.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/iol-investment-asset-type")]
public class IOLInvestmentAssetTypeQueryController(IMapper mapper, IMediator mediator)
    : ApiBaseQueryController<IOLInvestmentAssetType?, IOLInvestmentAssetTypeEnum, IOLInvestmentAssetTypeDto>(mapper, mediator)
{
    [HttpGet]
    public async Task<IActionResult> GetTypes()
    => await Handle(new GetAllIOLInvestmentAssetTypesQuery());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTypeById(IOLInvestmentAssetTypeEnum id)
        => await Handle(new GetIOLInvestmentAssetTypeQuery { Id = id });
}
