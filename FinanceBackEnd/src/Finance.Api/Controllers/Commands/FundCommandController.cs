using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Api.Controllers.Requests;
using Finance.Application.Auth;
using Finance.Application.Legacy.Dtos.Funds;
using Finance.Application.Legacy.Mapping;
using Finance.Application.Services;
using Finance.Application.Services.Funds;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Funds;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/funds")]
public class FundCommandController(
    IMappingService mapper,
    IDispatcher<FinanceDispatchContext> dispatcher,
    FundService fundService)
    : CommandController<
        Fund,
        FundPermissions,
        CreateFundRequest,
        UpdateFundRequest,
        DeleteFundRequest,
        SetFundOwnerRequest,
        DeleteFundOwnerRequest,
        Guid,
        FundDto,
        FundService>(mapper, dispatcher, fundService)
{
}
