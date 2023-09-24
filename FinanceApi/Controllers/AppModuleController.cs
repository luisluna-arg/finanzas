using AutoMapper;
using FinanceApi.Application.Commands.AppModules;
using FinanceApi.Application.Dtos.AppModules;
using FinanceApi.Application.Queries.AppModules;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers;

[Route("api/app-modules")]
public class AppModuleController : ApiBaseController<AppModule?, Guid, AppModuleDto>
{
    public AppModuleController(IMapper mapper, IMediator mediator)
        : base(mapper, mediator)
    {
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetAllAppModulesQuery request)
        => await Handle(request);

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromQuery] GetAppModuleQuery request)
        => await Handle(request);

    [HttpPost]
    public async Task<IActionResult> Create(CreateAppModuleCommand command)
        => await Handle(command);

    [HttpPut]
    public async Task<IActionResult> Update(UpdateAppModuleCommand command)
        => await Handle(command);

    [HttpPatch("activate/{id}")]
    public async Task<IActionResult> Activate(Guid id)
        => await Handle(new ActivateAppModuleCommand { Id = id });

    [HttpPatch("deactivate/{id}")]
    public async Task<IActionResult> Deactivate(Guid id)
        => await Handle404(new DeactivateAppModuleCommand { Id = id });
}
