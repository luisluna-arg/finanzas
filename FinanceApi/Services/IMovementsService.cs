using FinanceApi.Dtos.Movement;

namespace FinanceApi.Services;

public interface IMovementsService
{
    Task<MovementDto[]> GetAll();
    Task<MovementDto?> GetById(Guid id);
    Task Create(CreateMovementDto movement);
    Task Update(MovementDto movement);
    Task Delete(Guid id);
    Task<TotalsDto?> GetTotals();
    Task<bool> Exists(Guid id);
}
