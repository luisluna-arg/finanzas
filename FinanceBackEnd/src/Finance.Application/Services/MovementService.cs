using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commands.Movements;
using Finance.Application.Services.Movements;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Movements;
using FinanceBackEnd.Finance.Domain.Enums;
using Finance.Persistence;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Services;

public class MovementService(
    IDispatcher<FinanceDispatchContext> dispatcher,
    FinanceDbContext dbContext)
    : ICRUDService<
        Movement,
        Guid,
        MovementPermissions,
        CreateMovementRequest,
        UpdateMovementRequest,
        DeleteMovementRequest>
{
    public async Task<DataResult<Movement>> Create(CreateMovementRequest request, HttpRequest? httpRequest = null)
    {
        await using var tx = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var result = await dispatcher.DispatchAsync(
                new CreateMovementCommand
                {
                    AppModuleId = request.AppModuleId,
                    CurrencyId = request.CurrencyId,
                    TimeStamp = request.TimeStamp,
                    Concept1 = request.Concept1,
                    Concept2 = request.Concept2,
                    Amount = request.Amount,
                    Total = request.Total,
                });

            if (!result.IsSuccess)
            {
                await tx.RollbackAsync();
                return result;
            }

            await dispatcher.DispatchAsync(
                new CreateMovementPermissionsCommand
                {
                    ResourceId = result.Data!.Id,
                    PermissionLevels = [PermissionLevelEnum.Owner],
                },
                httpRequest);

            await tx.CommitAsync();
            return result;
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return DataResult<Movement>.Failure(ex.Message);
        }
    }

    public async Task<DataResult<Movement>> Update(UpdateMovementRequest request, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new PartialUpdateMovementCommand
            {
                Id = request.Id,
                TimeStamp = request.TimeStamp,
                Concept1 = request.Concept1,
                Concept2 = request.Concept2,
                Amount = request.Amount,
                Total = request.Total,
            });

    public async Task<CommandResult> Delete(DeleteMovementRequest request, HttpRequest? httpRequest = null)
    {
        await using var tx = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var deleteResult = await dispatcher.DispatchCommandAsync(
                new DeleteMovementsCommand { Ids = request.Ids });

            deleteResult.ThrowIfFailed($"Type {typeof(Movement).Name} delete operation failed");

            foreach (var id in request.Ids)
            {
                var ownershipDeleteResult = await dispatcher.DispatchAsync(
                    new DeleteMovementOwnerCommand { EntityId = id },
                    httpRequest);

                ownershipDeleteResult.ThrowIfFailed($"Type {typeof(Movement).Name} owner delete operation failed");
            }

            await tx.CommitAsync();
            return CommandResult.Success();
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return CommandResult.Failure(ex.Message);
        }
    }

    public async Task<DataResult<MovementPermissions>> SetOwner(Guid resourceId, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new CreateMovementPermissionsCommand
            {
                ResourceId = resourceId,
                PermissionLevels = [PermissionLevelEnum.Owner],
            },
            httpRequest);

    public async Task<CommandResult> DeleteOwner(Guid resourceId, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new DeleteMovementOwnerCommand { EntityId = resourceId },
            httpRequest);

    public async Task<CommandResult> Activate(Guid[] ids, HttpRequest? httpRequest = null)
    {
        foreach (var id in ids)
            await dispatcher.DispatchCommandAsync(new ActivateMovementCommand { Id = id });
        return CommandResult.Success();
    }

    public async Task<CommandResult> Deactivate(Guid[] ids, HttpRequest? httpRequest = null)
    {
        foreach (var id in ids)
            await dispatcher.DispatchCommandAsync(new DeactivateMovementCommand { Id = id });
        return CommandResult.Success();
    }
}
