using Finance.Api.Controllers.Base;
using Finance.Application.Dtos.Identities;
using Finance.Application.Mapping;
using Finance.Application.Queries.Identities;
using Finance.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/identities")]
public class IdentityQueryController(IMappingService mapper, IMediator mediator)
    : BasicQueryController<Identity?, Guid, IdentityDto, GetAllIdentitiesQuery, GetIdentityQuery>(mapper, mediator);
