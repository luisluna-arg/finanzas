using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Auth;
using Finance.Application.Legacy.Dtos.AppModules;
using Finance.Application.Legacy.Mapping;
using Finance.Application.Legacy.Queries.AppModules;
using Finance.Domain.Enums;
using Finance.Domain.Models.AppModules;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Queries;

[Route("api/app-module-types")]
public class AppModuleTypeQueryController(IMappingService mapper, IDispatcher<FinanceDispatchContext> dispatcher)
    : ApiBaseQueryController<AppModuleType, AppModuleTypeEnum, AppModuleTypeDto>(mapper, dispatcher)
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetAllAppModuleTypesQuery request)
        => await ExecuteAsync(request);

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromQuery] GetAppModuleTypeQuery request)
        => await ExecuteAsync(request);
}
