using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Dtos.Currencies;
using Finance.Application.Mapping;
using Finance.Application.Queries.Currencies;
using Finance.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/currencies")]
public class CurrencyQueryController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : BasicQueryController<Currency, Guid, CurrencyDto, GetAllCurrenciesQuery, GetCurrencyQuery>(mapper, dispatcher);
