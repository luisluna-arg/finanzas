using Finance.Api.Controllers.Base;
using Finance.Application.Dtos.Frequencies;
using Finance.Application.Mapping;
using Finance.Application.Queries.Frequencies;
using Finance.Domain.Enums;
using Finance.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/frequencies")]
public class FrequencyQueryController(IMappingService mapper, IMediator mediator)
    : BasicQueryController<Frequency?, FrequencyEnum, FrequencyDto, GetAllFrequenciesQuery, GetFrequencyQuery>(mapper, mediator);
