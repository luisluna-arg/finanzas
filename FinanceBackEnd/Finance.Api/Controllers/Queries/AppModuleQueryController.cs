using Finance.Api.Controllers.Base;
using Finance.Application.Dtos.AppModules;
using Finance.Application.Mapping;
using Finance.Application.Queries.AppModules;
using Finance.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/app-modules")]
public class AppModuleQueryController(IMappingService mapper, IMediator mediator)
    : ApiBaseQueryController<AppModule, Guid, AppModuleDto>(mapper, mediator)
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetAllAppModulesQuery request)
        => await Handle(request);

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromQuery] GetAppModuleQuery request)
        => await Handle(request);
}
