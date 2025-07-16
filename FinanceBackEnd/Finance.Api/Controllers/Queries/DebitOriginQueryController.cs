using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Dtos.DebitOrigins;
using Finance.Application.Mapping;
using Finance.Application.Queries.DebitOrigins;
using Finance.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/debit-origins")]
public class DebitOriginQueryController(IMappingService mapper, IDispatcher dispatcher)
    : BasicQueryController<DebitOrigin, Guid, DebitOriginDto, GetAllDebitOriginsQuery, GetDebitOriginQuery>(mapper, dispatcher);
