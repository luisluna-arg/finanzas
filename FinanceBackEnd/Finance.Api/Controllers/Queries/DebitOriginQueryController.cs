using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Dtos.DebitOrigins;
using Finance.Application.Mapping;
using Finance.Application.Queries.DebitOrigins;
using Finance.Domain.Models.Debits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/debit-origins")]
[Authorize(Policy = "AdminOrOwnerPolicy")]
public class DebitOriginQueryController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : BasicQueryController<DebitOrigin, Guid, DebitOriginDto, GetAllDebitOriginsQuery, GetDebitOriginQuery>(mapper, dispatcher);
