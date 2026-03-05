using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Legacy.Dtos.Currencies;
using Finance.Application.Legacy.Mapping;
using Finance.Application.Legacy.Queries.Currencies;
using Finance.Domain.Models.Currencies;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/currencies")]
public class CurrencyQueryController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : BasicQueryController<Currency, Guid, CurrencyDto, GetAllCurrenciesQuery, GetCurrencyQuery>(mapper, dispatcher);
