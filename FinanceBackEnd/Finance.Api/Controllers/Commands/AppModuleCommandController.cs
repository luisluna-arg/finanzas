using Finance.Api.Controllers.Base;
using Finance.Application.Commands.AppModules;
using Finance.Application.Dtos.AppModules;
using Finance.Application.Mapping;
using Finance.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/app-modules")]
public class AppModuleCommandController(IMappingService mapper, IMediator mediator)
    : ApiBaseCommandController<AppModule?, Guid, AppModuleDto>(mapper, mediator)
{
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
