using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commands.Incomes;
using Finance.Application.Services.Incomes;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Incomes;
using Finance.Persistence;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Services;

public class IncomeService(
    IDispatcher<FinanceDispatchContext> dispatcher,
    FinanceDbContext dbContext)
    : ICRUDService<
        Income,
        Guid,
        IncomePermissions,
        CreateIncomeRequest,
        UpdateIncomeRequest,
        DeleteIncomeRequest>
{
    public async Task<DataResult<Income>> Create(CreateIncomeRequest request, HttpRequest? httpRequest = null)
    {
        await using var tx = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var result = await dispatcher.DispatchAsync(
                new CreateIncomeCommand
                {
                    BankId = request.BankId,
                    CurrencyId = request.CurrencyId,
                    TimeStamp = request.TimeStamp,
                    Amount = request.Amount,
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
            return DataResult<Income>.Failure(ex.Message);
        }
    }

    public async Task<DataResult<Income>> Update(UpdateIncomeRequest request, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new UpdateIncomeCommand
            {
                Id = request.Id,
                BankId = request.BankId,
                CurrencyId = request.CurrencyId,
                TimeStamp = request.TimeStamp,
                Amount = request.Amount,
            });

    public async Task<CommandResult> Delete(DeleteIncomeRequest request, HttpRequest? httpRequest = null)
    {
        await using var tx = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var deleteResult = await dispatcher.DispatchAsync(
                new DeleteIncomesCommand() { Ids = request.Ids },
                httpRequest);

            deleteResult.ThrowIfFailed($"Type {typeof(Income).Name} delete operation failed");

            foreach (var id in request.Ids)
            {
                var ownershipDeleteResult = await dispatcher.DispatchAsync(
                    new DeleteIncomeOwnerCommand { EntityId = id },
                    httpRequest);

                ownershipDeleteResult.ThrowIfFailed($"Type {typeof(Income).Name} owner delete operation failed");
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

    public async Task<DataResult<IncomePermissions>> SetOwner(Guid resourceId, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new CreateIncomePermissionsCommand
            {
                ResourceId = resourceId,
                PermissionLevels = [PermissionLevelEnum.Owner],
            },
            httpRequest);

    public async Task<CommandResult> DeleteOwner(Guid resourceId, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new DeleteIncomeOwnerCommand { EntityId = resourceId },
            httpRequest);

    public async Task<CommandResult> Activate(Guid[] ids, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new ActivateIncomeCommand { Ids = ids },
            httpRequest);

    public async Task<CommandResult> Deactivate(Guid[] ids, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new DeactivateIncomeCommand { Ids = ids },
            httpRequest);
}
