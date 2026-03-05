using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Legacy.Dtos.Identities;
using Finance.Application.Legacy.Mapping;
using Finance.Application.Legacy.Queries.Identities;
using Finance.Domain.Models.Identities;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/identities")]
public class IdentityQueryController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : BasicQueryController<Identity, Guid, IdentityDto, GetAllIdentitiesQuery, GetIdentityQuery>(mapper, dispatcher);
