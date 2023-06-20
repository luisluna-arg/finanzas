using FinanceApi.Application.Dtos.Movements;
using FinanceApi.Domain.Models;

namespace FinanceApi.Application.Dtos.Factories;

public interface IMovementDtoFactory
{
    MovementDto[] Create(IEnumerable<Movement> movements);
    MovementDto Create(Movement movement);
    TotalsDto CreateTotals(Movement movement);
}
