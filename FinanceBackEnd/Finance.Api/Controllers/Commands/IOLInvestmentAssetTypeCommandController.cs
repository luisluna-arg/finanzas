using AutoMapper;
using Finance.Api.Controllers.Base;
using Finance.Application.Commands.IOLInvestments;
using Finance.Application.Dtos.IOLInvestmentAssetTypes;
using Finance.Domain.Enums;
using Finance.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/iol-investment-asset-type")]
public class IOLInvestmentAssetTypeCommandController(IMapper mapper, IMediator mediator)
    : ApiBaseCommandController<IOLInvestmentAssetType?, IOLInvestmentAssetTypeEnum, IOLInvestmentAssetTypeDto>(mapper, mediator)
{
    [HttpPatch("activate/{id}")]
    public async Task<IActionResult> Activate(IOLInvestmentAssetTypeEnum id)
        => await Handle(new ActivateIOLInvestmentAssetTypeCommand { Id = id });

    [HttpPatch("deactivate/{id}")]
    public async Task<IActionResult> Deactivate(IOLInvestmentAssetTypeEnum id)
        => await Handle404(new DeactivateIOLInvestmentAssetTypeCommand { Id = id });
}
