using AutoMapper;
using FinanceApi.Application.Dtos.Frequencies;
using FinanceApi.Application.Queries.Frequencies;
using FinanceApi.Controllers.Base;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Queries;

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
