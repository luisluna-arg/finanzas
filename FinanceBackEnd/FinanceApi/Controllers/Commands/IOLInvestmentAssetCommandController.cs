using AutoMapper;
using FinanceApi.Application.Commands.IOLInvestments;
using FinanceApi.Application.Dtos.IOLInvestmentAssets;
using FinanceApi.Controllers.Base;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Commands;

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
