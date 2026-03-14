using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Api.Controllers.Requests;
using Finance.Application.Auth;
using Finance.Application.Legacy.Dtos.IOLInvestmentAssets;
using Finance.Application.Legacy.Mapping;
using Finance.Application.Services;
using Finance.Application.Services.IOLInvestmentAssets;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.IOLInvestments;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/iol-investment-asset")]
public class IOLInvestmentAssetCommandController(
    IMappingService mapper,
    IDispatcher<FinanceDispatchContext> dispatcher,
    IOLInvestmentAssetService iolInvestmentAssetService)
    : CommandController<
        IOLInvestmentAsset,
        IOLInvestmentAssetPermissions,
        CreateIOLInvestmentAssetRequest,
        UpdateIOLInvestmentAssetRequest,
        DeleteIOLInvestmentAssetRequest,
        SetIOLInvestmentAssetOwnerRequest,
        DeleteIOLInvestmentAssetOwnerRequest,
        Guid,
        IOLInvestmentAssetDto,
        IOLInvestmentAssetService>(mapper, dispatcher, iolInvestmentAssetService)
{
}
