using FinanceApi.Application.Base.Handlers;
using FinanceApi.Core.SpecialTypes;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;
using MediatR;

namespace FinanceApi.Application.Commands.Movements;

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
        var movement = await movementRepository.GetByIdAsync(command.Id, cancellationToken);
        if (movement == null) throw new Exception("Movement not found");

        movement.Amount = command.Amount;
        movement.Concept1 = command.Concept1;
        movement.Concept2 = command.Concept2;
        movement.TimeStamp = command.TimeStamp;
        movement.Total = command.Total;

        await movementRepository.UpdateAsync(movement, cancellationToken);

        return await Task.FromResult(movement);
    }
}

public class PartialUpdateMovementCommand : IRequest<Movement>
{
    required public Guid Id { get; set; }

    required public DateTime TimeStamp { get; set; }

    required public string Concept1 { get; set; }

    required public string? Concept2 { get; set; }

    required public Money Amount { get; set; }

    required public Money? Total { get; set; }
}
