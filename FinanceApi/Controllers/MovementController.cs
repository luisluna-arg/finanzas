using FinanceApi.Application.Dtos.Movement;
using FinanceApi.Commons;
using FinanceApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers;

[ApiController]
[Route("movements")]
public class MovementController : ControllerBase
{
    private readonly IMovementsService service;

    public MovementController(IMovementsService movementService)
    {
        service = movementService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovementDto>>> Get()
    {
        return await service.GetAll();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MovementDto>> GetById(Guid id)
    {
        var movement = await service.GetById(id);

        if (movement == null) return NotFound();

        return movement;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateMovementDto movement)
    {
        await service.Create(movement);

        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> Update(MovementDto movement)
    {
        if (!await service.Exists(movement.Id)) return NotFound();

        await service.Update(movement);

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!await service.Exists(id)) return NotFound();

        await service.Delete(id);

        return Ok();
    }

    [HttpGet("totals")]
    public async Task<IActionResult> Totals()
    {
        var totals = await service.GetTotals();

        if (totals == null) return Ok(new OkResponse("No funds available", false));

        return Ok(new TotalsDto());
    }
}
