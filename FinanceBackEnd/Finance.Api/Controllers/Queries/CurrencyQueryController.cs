using AutoMapper;
using Finance.Api.Controllers.Base;
using Finance.Application.Dtos.Currencies;
using Finance.Application.Queries.Currencies;
using Finance.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/currencies")]
public class CurrencyQueryController(IMapper mapper, IMediator mediator)
    : BasicQueryController<Currency?, Guid, CurrencyDto, GetAllCurrenciesQuery, GetCurrencyQuery>(mapper, mediator);
