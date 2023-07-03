using AutoMapper;
using FinanceApi.Application.Commands.Movements;
using FinanceApi.Application.Dtos.Movements;
using FinanceApi.Application.Queries.Movements;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers;

[Route("api/movements")]
public class MovementController : ApiBaseController<Movement, MovementDto>
{
    public MovementController(IMapper mapper, IMediator mediator)
        : base(mapper, mediator)
    {
    }

    [HttpGet]
    public async Task<IActionResult> Get()
        => await Handle(new GetAllMovementsQuery());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
        => await Handle(new GetMovementQuery { Id = id });

    [HttpPost]
    public async Task<IActionResult> Create(CreateMovementCommand command)
        => await Handle(command);

    [HttpPut]
    public async Task<IActionResult> Update(PartialUpdateMovementCommand command)
        => await Handle(command);
}
