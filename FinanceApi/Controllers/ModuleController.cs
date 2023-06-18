using AutoMapper;
using FinanceApi.Application.Commands.Modules;
using FinanceApi.Application.Dtos.Module;
using FinanceApi.Application.Queries.Modules;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers;

[ApiController]
[Route("api/modules")]
public class ModuleController : ControllerBase
{
    // private readonly IModulesService service;
    private readonly IMapper mapper;
    private readonly IMediator mediator;

    public ModuleController(IMapper mapper, IMediator mediator)
    {
        this.mapper = mapper;
        this.mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
        => Ok(mapper.Map<ModuleDto[]>(await mediator.Send(new GetAllModulesQuery())));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
        => Ok(mapper.Map<ModuleDto>(await mediator.Send(new GetModuleQuery { Id = id })));

    [HttpPost]
    public async Task<IActionResult> Create(CreateModuleCommand command)
        => Ok(mapper.Map<ModuleDto>(await mediator.Send(command)));

    [HttpPut]
    public async Task<IActionResult> Update(UpdateModuleCommand command)
        => Ok(mapper.Map<ModuleDto>(await mediator.Send(command)));
}
