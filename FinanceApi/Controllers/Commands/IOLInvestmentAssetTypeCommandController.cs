using AutoMapper;
using FinanceApi.Application.Commands.IOLInvestments;
using FinanceApi.Application.Dtos.IOLInvestmentAssetTypes;
using FinanceApi.Controllers.Base;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Commands;

[Route("api/iol-investment-asset-type")]
public class IOLInvestmentAssetTypeCommandController(IMapper mapper, IMediator mediator)
    : ApiBaseCommandController<IOLInvestmentAssetType?, ushort, IOLInvestmentAssetTypeDto>(mapper, mediator)
{
    [HttpPatch("activate/{id}")]
    public async Task<IActionResult> Activate(ushort id)
        => await Handle(new ActivateIOLInvestmentAssetTypeCommand { Id = id });

    [HttpPatch("deactivate/{id}")]
    public async Task<IActionResult> Deactivate(ushort id)
        => await Handle404(new DeactivateIOLInvestmentAssetTypeCommand { Id = id });
}
