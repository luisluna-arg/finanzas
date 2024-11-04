using AutoMapper;
using Finance.Api.Controllers.Base;
using Finance.Application.Commands.IOLInvestments;
using Finance.Application.Dtos.IOLInvestmentAssets;
using Finance.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/iol-investment-asset")]
public class IOLInvestmentAssetCommandController(IMapper mapper, IMediator mediator)
    : ApiBaseCommandController<IOLInvestmentAsset?, Guid, IOLInvestmentAssetDto>(mapper, mediator)
{
    [HttpPatch("activate/{id}")]
    public async Task<IActionResult> Activate(Guid id)
        => await Handle(new ActivateIOLInvestmentAssetCommand { Id = id });

    [HttpPatch("deactivate/{id}")]
    public async Task<IActionResult> Deactivate(Guid id)
        => await Handle404(new DeactivateIOLInvestmentAssetCommand { Id = id });
}
