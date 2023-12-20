using AutoMapper;
using FinanceApi.Application.Dtos.AppModules;
using FinanceApi.Application.Queries.AppModules;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers;

[Route("api/app-module-types")]
public class AppModuleTypeController : ApiBaseController<AppModuleType?, short, AppModuleTypeDto>
{
    public AppModuleTypeController(IMapper mapper, IMediator mediator)
        : base(mapper, mediator)
    {
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetAllAppModuleTypesQuery request)
        => await Handle(request);

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromQuery] GetAppModuleTypeQuery request)
        => await Handle(request);
}
