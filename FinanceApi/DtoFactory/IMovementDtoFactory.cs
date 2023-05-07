using FinanceApi.Dtos.Movement;
using FinanceApi.Models;

namespace FinanceApi.DtoFactory;

public interface IMovementDtoFactory
{
    MovementDto[] Create(IEnumerable<Movement> movements);
    MovementDto Create(Movement movement);
    TotalsDto CreateTotals(Movement movement);
}
