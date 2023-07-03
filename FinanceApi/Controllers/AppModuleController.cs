using AutoMapper;
using FinanceApi.Application.Commands.AppModules;
using FinanceApi.Application.Dtos.AppModules;
using FinanceApi.Application.Queries.AppModules;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers;

[Route("api/app-modules")]
public class AppModuleController : ApiBaseController<AppModule, Guid, AppModuleDto>
{
    public AppModuleController(IMapper mapper, IMediator mediator)
        : base(mapper, mediator)
    {
    }

    [HttpGet]
    public async Task<IActionResult> Get()
        => await Handle(new GetAllAppModulesQuery());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
        => await Handle(new GetAppModuleQuery { Id = id });

    [HttpPost]
    public async Task<IActionResult> Create(CreateAppModuleCommand command)
        => await Handle(command);

    [HttpPut]
    public async Task<IActionResult> Update(UpdateAppModuleCommand command)
        => await Handle(command);
}
