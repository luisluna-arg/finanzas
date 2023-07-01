using FinanceApi.Application.Dtos.Factories;
using FinanceApi.Application.Dtos.Movements;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Services;

public class MovementsService : IMovementsService
{
    private readonly FinanceDbContext dbContext;
    private readonly IMovementDtoFactory dtoFactory;

    public MovementsService(FinanceDbContext db, IMovementDtoFactory movementDtoFactory)
    {
        dbContext = db;
        dtoFactory = movementDtoFactory;
    }

    public async Task<MovementDto[]> GetAll()
    {
        return dtoFactory.Create(await dbContext.Movement.ToArrayAsync());
    }

    public async Task<MovementDto?> GetById(Guid id)
    {
        var movement = await GetRecordById(id, false);
        return movement != null ? dtoFactory.Create(movement) : null;
    }

    public async Task Create(CreateMovementDto movement)
    {
        var appModule = await GetAppModule();

        var newMovement = new Movement()
        {
            AppModule = appModule,
            Amount = movement.Amount,
            Concept1 = movement.Concept1,
            Concept2 = movement.Concept2,
            TimeStamp = movement.TimeStamp,
            Total = movement.Total,
        };

        dbContext.Movement.Add(newMovement);
        await dbContext.SaveChangesAsync();
    }

    public async Task Update(MovementDto movement)
    {
        var existingMovement = await GetRecordById(movement.Id);

        existingMovement.Amount = movement.Amount;
        existingMovement.Concept1 = movement.Concept1;
        existingMovement.Concept2 = movement.Concept2;
        existingMovement.TimeStamp = movement.TimeStamp;
        existingMovement.Total = movement.Total;

        dbContext.Movement.Update(existingMovement);
        await dbContext.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var existingMovement = await GetRecordById(id);

        dbContext.Movement.Remove(existingMovement);
        await dbContext.SaveChangesAsync();
    }

    public async Task<TotalsDto?> GetTotals()
    {
        var latestMovement = await dbContext.Movement.OrderByDescending(o => o.TimeStamp).FirstOrDefaultAsync();

        if (latestMovement == null) return null;

        return dtoFactory.CreateTotals(latestMovement);
    }

    public async Task<bool> Exists(Guid id)
    {
        return await dbContext.Movement.AnyAsync(o => o.Id == id);
    }

    private async Task<AppModule> GetAppModule()
    {
        var appModule = await dbContext.AppModule.FirstOrDefaultAsync(o => o.Name == "Fondos");
        if (appModule == null) throw new Exception("Fund app module not found");
        return appModule;
    }

    private async Task<Movement> GetRecordById(Guid id, bool throwException = true)
    {
        var movement = await dbContext.Movement.FirstOrDefaultAsync(o => o.Id == id);
        if (movement == null && throwException) throw new Exception($"Movement {id} not found");
        return movement!;
    }
}
