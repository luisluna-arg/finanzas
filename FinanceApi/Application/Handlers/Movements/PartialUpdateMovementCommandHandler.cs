using FinanceApi.Application.Commands.Movements;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.Movements;

public class UpdateMovementCommandHandler : BaseResponseHandler<PartialUpdateMovementCommand, Movement>
{
    public UpdateMovementCommandHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<Movement> Handle(PartialUpdateMovementCommand command, CancellationToken cancellationToken)
    {
        Movement movement = await GetMovement(command.Id);

        movement.Amount = command.Amount;
        movement.Concept1 = command.Concept1;
        movement.Concept2 = command.Concept2;
        movement.TimeStamp = command.TimeStamp;
        movement.Total = command.Total;

        DbContext.Movement.Update(movement);
        await DbContext.SaveChangesAsync();

        return await Task.FromResult(movement);
    }

    private async Task<Movement> GetMovement(Guid id)
    {
        var movement = await DbContext.Movement.FirstOrDefaultAsync(o => o.Id == id);
        if (movement == null) throw new Exception("Movement not found");
        return movement;
    }
}
