using FinanceApi.Commons;
using FinanceApi.Dtos.Movement;
using FinanceApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Controllers;

[ApiController]
[Route("movements")]
public class MovementController : ControllerBase
{
    private readonly IMovementsService movementService;

    public MovementController(IMovementsService movementService)
    {
        this.movementService = movementService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovementDto>>> Get()
    {
        return await this.movementService.GetAll();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MovementDto>> GetById(Guid id)
    {
        var movement = await this.movementService.GetById(id);

        if (movement == null) return NotFound();

        return movement;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateMovementDto movement)
    {
        await this.movementService.Create(movement);

        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> Update(MovementDto movement)
    {
        if (!await this.movementService.Exists(movement.Id)) return NotFound();

        await this.movementService.Update(movement);

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!await this.movementService.Exists(id)) return NotFound();

        await this.movementService.Delete(id);

        return Ok();
    }

    [HttpGet("totals")]
    public async Task<IActionResult> Totals()
    {
        var totals = await this.movementService.GetTotals();

        if (totals == null) return Ok(new OkResponse("No funds available", false));

        return Ok(new TotalsDto());
    }
}
