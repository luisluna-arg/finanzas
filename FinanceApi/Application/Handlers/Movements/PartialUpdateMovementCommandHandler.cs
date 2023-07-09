using FinanceApi.Application.Commands.Movements;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Handlers.Movements;

public class UpdateMovementCommandHandler : BaseResponseHandler<PartialUpdateMovementCommand, Movement>
{
    private readonly IRepository<Movement, Guid> movementRepository;

    public UpdateMovementCommandHandler(
        FinanceDbContext db,
        IRepository<Movement, Guid> movementRepository)
        : base(db)
    {
        this.movementRepository = movementRepository;
    }

    public override async Task<Movement> Handle(PartialUpdateMovementCommand command, CancellationToken cancellationToken)
    {
        Movement movement = await movementRepository.GetById(command.Id);

        movement.Amount = command.Amount;
        movement.Concept1 = command.Concept1;
        movement.Concept2 = command.Concept2;
        movement.TimeStamp = command.TimeStamp;
        movement.Total = command.Total;

        await movementRepository.Update(movement);

        return await Task.FromResult(movement);
    }
}
