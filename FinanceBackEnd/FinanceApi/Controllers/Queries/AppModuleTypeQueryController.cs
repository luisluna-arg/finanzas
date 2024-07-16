using AutoMapper;
using FinanceApi.Application.Dtos.AppModules;
using FinanceApi.Application.Queries.AppModules;
using FinanceApi.Controllers.Base;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Queries;

[Route("api/app-module-types")]
public class AppModuleTypeQueryController(IMapper mapper, IMediator mediator) : ApiBaseQueryController<AppModuleType?, short, AppModuleTypeDto>(mapper, mediator)
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetAllAppModuleTypesQuery request)
        => await Handle(request);

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromQuery] GetAppModuleTypeQuery request)
        => await Handle(request);
}
