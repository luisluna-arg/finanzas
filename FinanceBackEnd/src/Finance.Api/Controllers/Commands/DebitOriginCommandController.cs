using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Api.Controllers.Requests;
using Finance.Application.Auth;
using Finance.Application.Legacy.Dtos.DebitOrigins;
using Finance.Application.Legacy.Mapping;
using Finance.Application.Services;
using Finance.Application.Services.DebitOrigins;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Debits;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/debit-origins")]
public class DebitOriginCommandController(
    IMappingService mapper,
    IDispatcher<FinanceDispatchContext> dispatcher,
    DebitOriginService debitOriginService)
    : CommandController<
        DebitOrigin,
        DebitOriginPermissions,
        CreateDebitOriginRequest,
        UpdateDebitOriginRequest,
        DeleteDebitOriginRequest,
        SetDebitOriginOwnerRequest,
        DeleteDebitOriginOwnerRequest,
        Guid,
        DebitOriginDto,
        DebitOriginService>(mapper, dispatcher, debitOriginService)
{
}
