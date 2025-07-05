using AutoMapper;
using Finance.Api.Controllers.Base;
using Finance.Application.Dtos.DebitOrigins;
using Finance.Application.Queries.DebitOrigins;
using Finance.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/debit-origins")]
public class DebitOriginQueryController(IMapper mapper, IMediator mediator)
    : BasicQueryController<DebitOrigin?, Guid, DebitOriginDto, GetAllDebitOriginsQuery, GetDebitOriginQuery>(mapper, mediator);
