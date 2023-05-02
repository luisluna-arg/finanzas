using FinanceApi.DtoFactory;
using FinanceApi.Dtos.Movement;
using FinanceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Services;

public class MovementsService : IMovementsService
{
    private readonly FinanceDbContext dbContext;
    private readonly IMovementDtoFactory movementDtoFactory;

    public MovementsService(FinanceDbContext db, IMovementDtoFactory movementDtoFactory)
    {
        this.dbContext = db;
        this.movementDtoFactory = movementDtoFactory;
    }

    public async Task<MovementDto[]> GetAll()
    {
        return movementDtoFactory.Create(await this.dbContext.Movement.ToArrayAsync());
    }

    public  async Task<MovementDto?> GetById(Guid id)
    {
        var movement = await GetRecordById(id, false);
        return movement != null ? movementDtoFactory.Create(movement) : null;
    }

    public async Task Create(CreateMovementDto movement)
    {
        var module = await GetModule();

        var newMovement = new Movement()
        {
            Module = module,
            Amount = movement.Amount,
            Concept1 = movement.Concept1,
            Concept2 = movement.Concept2,
            TimeStamp = movement.TimeStamp,
            Total = movement.Total,
        };

        this.dbContext.Movement.Add(newMovement);
        await this.dbContext.SaveChangesAsync();
    }

    public async Task Update(MovementDto movement)
    {
        var existingMovement = await GetRecordById(movement.Id);

        existingMovement.Amount = movement.Amount;
        existingMovement.Concept1 = movement.Concept1;
        existingMovement.Concept2 = movement.Concept2;
        existingMovement.TimeStamp = movement.TimeStamp;
        existingMovement.Total = movement.Total;

        this.dbContext.Movement.Update(existingMovement);
        await this.dbContext.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var existingMovement = await GetRecordById(id);

        this.dbContext.Movement.Remove(existingMovement);
        await this.dbContext.SaveChangesAsync();
    }

    public  async Task<TotalsDto?> GetTotals()
    {
        var latestMovement = await this.dbContext.Movement.OrderByDescending(o => o.TimeStamp).FirstOrDefaultAsync();

        if (latestMovement == null) return null;

        return movementDtoFactory.CreateTotals(latestMovement);
    }

    public async Task<bool> Exists(Guid id)
    {
        return await this.dbContext.Movement.AnyAsync(o => o.Id == id);
    }

    private async Task<Module> GetModule()
    {
        var module = await this.dbContext.Module.FirstOrDefaultAsync(o => o.Name == "Fondos");
        if (module == null) throw new Exception("Fund module not found");
        return module;
    }

    private async Task<Movement> GetRecordById(Guid id, bool throwException = true)
    {
        var movement = await this.dbContext.Movement.FirstOrDefaultAsync(o => o.Id == id);
        if (movement == null && throwException) throw new Exception($"Movement {id} not found");
        return movement!;
    }
}
