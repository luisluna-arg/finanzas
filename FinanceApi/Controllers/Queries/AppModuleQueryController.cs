using AutoMapper;
using FinanceApi.Application.Dtos.AppModules;
using FinanceApi.Application.Queries.AppModules;
using FinanceApi.Controllers.Base;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers.Queries;

[Route("api/app-modules")]
public class AppModuleQueryController(IMapper mapper, IMediator mediator)
    : ApiBaseQueryController<AppModule?, Guid, AppModuleDto>(mapper, mediator)
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetAllAppModulesQuery request)
        => await Handle(request);

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromQuery] GetAppModuleQuery request)
        => await Handle(request);
}
