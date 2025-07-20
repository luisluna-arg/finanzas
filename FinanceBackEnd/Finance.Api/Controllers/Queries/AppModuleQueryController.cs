using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Dtos.AppModules;
using Finance.Application.Mapping;
using Finance.Application.Queries.AppModules;
using Finance.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/app-modules")]
public class AppModuleQueryController(IMappingService mapper, IDispatcher dispatcher)
    : ApiBaseQueryController<AppModule, Guid, AppModuleDto>(mapper, dispatcher)
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetAllAppModulesQuery request)
        => await ExecuteAsync(request);

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromQuery] GetAppModuleQuery request)
        => await ExecuteAsync(request);
}
