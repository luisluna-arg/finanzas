using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Commands.IOLInvestments;
using Finance.Application.Legacy.Dtos.IOLInvestmentAssetTypes;
using Finance.Application.Legacy.Mapping;
using Finance.Domain.Enums;
using Finance.Domain.Models.IOLInvestments;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/iol-investment-asset-type")]
public class IOLInvestmentAssetTypeCommandController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : ApiBaseCommandController<IOLInvestmentAssetType?, IOLInvestmentAssetTypeEnum, IOLInvestmentAssetTypeDto>(mapper, dispatcher)
{
    [HttpPatch("activate/{id}")]
    public async Task<IActionResult> Activate(IOLInvestmentAssetTypeEnum id)
        => await ExecuteAsync(new ActivateIOLInvestmentAssetTypeCommand { Ids = [id] });

    [HttpPatch("deactivate/{id}")]
    public async Task<IActionResult> Deactivate(IOLInvestmentAssetTypeEnum id)
        => await ExecuteAsync(new DeactivateIOLInvestmentAssetTypeCommand { Ids = [id] });
}
