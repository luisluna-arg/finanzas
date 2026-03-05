using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Legacy.Dtos.Frequencies;
using Finance.Application.Legacy.Mapping;
using Finance.Application.Legacy.Queries.Frequencies;
using Finance.Domain.Enums;
using Finance.Domain.Models.Frequencies;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/frequencies")]
public class FrequencyQueryController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : BasicQueryController<Frequency, FrequencyEnum, FrequencyDto, GetAllFrequenciesQuery, GetFrequencyQuery>(mapper, dispatcher);
