using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Base.Handlers;
using Finance.Domain.SpecialTypes;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistence;

namespace Finance.Application.Commands.Movements;

public class UpdateMovementCommandHandler : BaseCommandHandler<PartialUpdateMovementCommand, Movement>
{
    private readonly IRepository<Movement, Guid> movementRepository;

    public UpdateMovementCommandHandler(
        FinanceDbContext db,
        IRepository<Movement, Guid> movementRepository)
        : base(db)
    {
        this.movementRepository = movementRepository;
    }

    public override async Task<DataResult<Movement>> ExecuteAsync(PartialUpdateMovementCommand command, CancellationToken cancellationToken)
    {
        var movement = await movementRepository.GetByIdAsync(command.Id, cancellationToken);
        if (movement == null) throw new Exception("Movement not found");

        movement.Amount = command.Amount;
        movement.Concept1 = command.Concept1;
        movement.Concept2 = command.Concept2;
        movement.TimeStamp = command.TimeStamp;
        movement.Total = command.Total;

        await movementRepository.UpdateAsync(movement, cancellationToken);

        return DataResult<Movement>.Success(movement);
    }
}

public class PartialUpdateMovementCommand : ICommand
{
    required public Guid Id { get; set; }

    required public DateTime TimeStamp { get; set; }

    required public string Concept1 { get; set; }

    required public string? Concept2 { get; set; }

    required public Money Amount { get; set; }

    required public Money? Total { get; set; }
}
