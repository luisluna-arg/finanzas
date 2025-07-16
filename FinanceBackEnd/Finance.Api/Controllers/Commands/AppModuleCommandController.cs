using CQRSDispatch.Interfaces;
using Finance.Api.Controllers.Base;
using Finance.Application.Commands.AppModules;
using Finance.Application.Dtos.AppModules;
using Finance.Application.Mapping;
using Finance.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Commands;

[Route("api/app-modules")]
public class AppModuleCommandController(IMappingService mapper, IDispatcher dispatcher)
    : ApiBaseCommandController<AppModule?, Guid, AppModuleDto>(mapper, dispatcher)
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateAppModuleCommand command)
        => await ExecuteAsync(command);

    [HttpPut]
    public async Task<IActionResult> Update(UpdateAppModuleCommand command)
        => await ExecuteAsync(command);

    [HttpPatch("activate/{id}")]
    public async Task<IActionResult> Activate(Guid id)
        => await ExecuteAsync(new ActivateAppModuleCommand { Id = id });

    [HttpPatch("deactivate/{id}")]
    public async Task<IActionResult> Deactivate(Guid id)
        => await Handle404Nullable(new DeactivateAppModuleCommand { Id = id });
}
