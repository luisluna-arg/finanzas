using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commands.Debits;
using Finance.Application.Services.Debits;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Debits;
using FinanceBackEnd.Finance.Domain.Enums;
using Finance.Persistence;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Services;

public class DebitService(
    IDispatcher<FinanceDispatchContext> dispatcher,
    FinanceDbContext dbContext)
    : ICRUDService<
        Debit,
        Guid,
        DebitPermissions,
        CreateDebitRequest,
        UpdateDebitRequest,
        DeleteDebitRequest>
{
    public async Task<DataResult<Debit>> Create(CreateDebitRequest request, HttpRequest? httpRequest = null)
    {
        await using var tx = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var result = await dispatcher.DispatchAsync(
                new CreateDebitCommand
                {
                    AppModuleId = request.AppModuleId,
                    Origin = request.Origin,
                    Amount = request.Amount,
                    Deactivated = request.Deactivated,
                    Frequency = request.Frequency,
                },
                httpRequest);

            if (!result.IsSuccess)
            {
                await tx.RollbackAsync();
                return result;
            }

            await tx.CommitAsync();
            return result;
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return DataResult<Debit>.Failure(ex.Message);
        }
    }

    public async Task<DataResult<Debit>> Update(UpdateDebitRequest request, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new UpdateDebitCommand
            {
                Id = request.Id,
                AppModuleId = request.AppModuleId,
                Origin = request.Origin,
                Amount = request.Amount,
                Deactivated = request.Deactivated,
                Frequency = request.Frequency,
            },
            httpRequest);

    public async Task<CommandResult> Delete(DeleteDebitRequest request, HttpRequest? httpRequest = null)
    {
        await using var tx = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var deleteResult = await dispatcher.DispatchAsync(
                new DeleteDebitCommand() { Ids = request.Ids },
                httpRequest);

            deleteResult.ThrowIfFailed($"Type {typeof(Debit).Name} delete operation failed");

            foreach (var id in request.Ids)
            {
                var ownershipDeleteResult = await dispatcher.DispatchAsync(
                    new DeleteDebitOwnerCommand { EntityId = id },
                    httpRequest);

                ownershipDeleteResult.ThrowIfFailed($"Type {typeof(Debit).Name} owner delete operation failed");
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

    public async Task<DataResult<DebitPermissions>> SetOwner(Guid resourceId, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new CreateDebitPermissionsCommand
            {
                ResourceId = resourceId,
                PermissionLevels = [PermissionLevelEnum.Owner],
            },
            httpRequest);

    public async Task<CommandResult> DeleteOwner(Guid resourceId, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new DeleteDebitOwnerCommand { EntityId = resourceId },
            httpRequest);

    public async Task<CommandResult> Activate(Guid[] ids, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new ActivateDebitCommand { Ids = ids },
            httpRequest);

    public async Task<CommandResult> Deactivate(Guid[] ids, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new DeactivateDebitCommand { Ids = ids },
            httpRequest);
}
