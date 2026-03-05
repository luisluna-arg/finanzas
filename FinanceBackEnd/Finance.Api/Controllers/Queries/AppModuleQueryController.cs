using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Legacy.Dtos.AppModules;
using Finance.Application.Legacy.Mapping;
using Finance.Application.Legacy.Queries.AppModules;
using Finance.Domain.Models.AppModules;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/app-modules")]
public class AppModuleQueryController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : ApiBaseQueryController<AppModule, Guid, AppModuleDto>(mapper, dispatcher)
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetAllAppModulesQuery request)
        => await ExecuteAsync(request);

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromQuery] GetAppModuleQuery request)
        => await ExecuteAsync(request);
}
