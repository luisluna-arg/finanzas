using Finance.Api.Controllers.Base;
using Finance.Application.Dtos.AppModules;
using Finance.Application.Mapping;
using Finance.Application.Queries.AppModules;
using Finance.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/app-module-types")]
public class AppModuleTypeQueryController(IMappingService mapper, IMediator mediator) : ApiBaseQueryController<AppModuleType?, short, AppModuleTypeDto>(mapper, mediator)
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetAllAppModuleTypesQuery request)
        => await Handle(request);

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromQuery] GetAppModuleTypeQuery request)
        => await Handle(request);
}
