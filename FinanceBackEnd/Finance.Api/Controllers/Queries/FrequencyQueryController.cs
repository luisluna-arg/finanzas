using AutoMapper;
using Finance.Api.Controllers.Base;
using Finance.Application.Dtos.Frequencies;
using Finance.Application.Queries.Frequencies;
using Finance.Domain.Enums;
using Finance.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/frequencies")]
public class FrequencyQueryController(IMapper mapper, IMediator mediator)
    : ApiBaseQueryController<Frequency?, FrequencyEnum, FrequencyDto>(mapper, mediator)
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetAllFrequenciesQuery request)
        => await Handle(request);

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromQuery] GetFrequencyQuery request)
    {
        return await Handle(request);
    }
}
