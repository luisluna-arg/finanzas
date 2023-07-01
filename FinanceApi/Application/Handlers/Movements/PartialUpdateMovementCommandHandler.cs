using FinanceApi.Application.Commands.Movements;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.Movements;

public class UpdateMovementCommandHandler : IRequestHandler<PartialUpdateMovementCommand, Movement>
{
    private readonly FinanceDbContext dbContext;

    public UpdateMovementCommandHandler(FinanceDbContext db)
    {
        dbContext = db;
    }

    public async Task<Movement> Handle(PartialUpdateMovementCommand command, CancellationToken cancellationToken)
    {
        Movement movement = await GetMovement(command.Id);

        movement.Amount = command.Amount;
        movement.Concept1 = command.Concept1;
        movement.Concept2 = command.Concept2;
        movement.TimeStamp = command.TimeStamp;
        movement.Total = command.Total;

        dbContext.Movement.Update(movement);
        await dbContext.SaveChangesAsync();

        return await Task.FromResult(movement);
    }

    private async Task<Movement> GetMovement(Guid id)
    {
        var Movement = await dbContext.Movement.FirstOrDefaultAsync(o => o.Id == id);
        if (Movement == null) throw new Exception("Movement not found");
        return Movement;
    }
}
