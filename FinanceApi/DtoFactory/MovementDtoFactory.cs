using FinanceApi.Dtos.Movement;
using FinanceApi.Models;

namespace FinanceApi.DtoFactory;

public class MovementDtoFactory : IMovementDtoFactory
{
    public MovementDto Create(Movement movement)
    {
        return new MovementDto()
        {
            Id = movement.Id,
            ModuleId = movement.ModuleId,
            CurrencyId = movement.CurrencyId,
            TimeStamp = movement.TimeStamp,
            Concept1 = movement.Concept1,
            Concept2 = movement.Concept2,
            Amount = movement.Amount,
            Total = movement.Total
        };
    }

    public MovementDto[] Create(IEnumerable<Movement> movements)
        => movements.Select(o => Create(o)).ToArray();

    public TotalsDto CreateTotals(Movement movement)
        => new TotalsDto()
        {
            Funds = movement.Total ?? 0,
            LastDeposit = movement.Total ?? 0,
            TimeStamp = movement.TimeStamp,
            Income = 0,
        };
}
