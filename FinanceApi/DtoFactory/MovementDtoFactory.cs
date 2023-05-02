using FinanceApi.Dtos.Movement;
using FinanceApi.Models;

namespace FinanceApi.DtoFactory;

public class MovementDtoFactory : IMovementDtoFactory
{
    public MovementDto[] Create(IEnumerable<Movement> movements) {
        return movements.Select(o => Create(o)).ToArray();
    }

    public MovementDto Create(Movement o)
    {
        return new MovementDto() {
            Id = o.Id,
            ModuleId = o.ModuleId,
            CurrencyId = o.CurrencyId,
            TimeStamp = o.TimeStamp,
            Concept1 = o.Concept1,
            Concept2 = o.Concept2,
            Amount = o.Amount,
            Total = o.Total
        };
    }

    public TotalsDto CreateTotals(Movement movement) 
    {
        return new TotalsDto() {
            Funds = movement.Total ?? 0,
            LastDeposit = movement.Total ?? 0,
            TimeStamp = movement.TimeStamp,
            Income = 0,
        };
    }
}