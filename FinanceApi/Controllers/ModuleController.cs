using AutoMapper;
using FinanceApi.Application.Commands.AppModules;
using FinanceApi.Application.Dtos.AppModules;
using FinanceApi.Application.Queries.Modules;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers;

[ApiController]
[Route("api/modules")]
public class ModuleController : ControllerBase
{
    private readonly IMapper mapper;
    private readonly IMediator mediator;

    public ModuleController(IMapper mapper, IMediator mediator)
    {
        this.mapper = mapper;
        this.mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
        => Ok(mapper.Map<AppModuleDto[]>(await mediator.Send(new GetAllModulesQuery())));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
        => Ok(mapper.Map<AppModuleDto>(await mediator.Send(new GetModuleQuery { Id = id })));

    [HttpPost]
    public async Task<IActionResult> Create(CreateAppModuleCommand command)
        => Ok(mapper.Map<AppModuleDto>(await mediator.Send(command)));

    [HttpPut]
    public async Task<IActionResult> Update(UpdateAppModuleCommand command)
        => Ok(mapper.Map<AppModuleDto>(await mediator.Send(command)));
}
