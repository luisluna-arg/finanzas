using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Dtos.Identities;
using Finance.Application.Mapping;
using Finance.Application.Queries.Identities;
using Finance.Domain.Models.Identities;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/identities")]
public class IdentityQueryController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : BasicQueryController<Identity, Guid, IdentityDto, GetAllIdentitiesQuery, GetIdentityQuery>(mapper, dispatcher);
