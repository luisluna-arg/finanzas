using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Commands.IOLInvestments;
using Finance.Application.Dtos.IOLInvestmentAssets;
using Finance.Application.Mapping;
using Finance.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/iol-investment-asset")]
public class IOLInvestmentAssetCommandController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : ApiBaseCommandController<IOLInvestmentAsset?, Guid, IOLInvestmentAssetDto>(mapper, dispatcher)
{
    [HttpPatch("activate/{id}")]
    public async Task<IActionResult> Activate(Guid id)
        => await Handle404(new ActivateIOLInvestmentAssetCommand { Id = id });

    [HttpPatch("deactivate/{id}")]
    public async Task<IActionResult> Deactivate(Guid id)
        => await Handle404(new DeactivateIOLInvestmentAssetCommand { Id = id });
}
