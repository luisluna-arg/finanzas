using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commands;
using Finance.Application.Commands.Funds;
using Finance.Application.Services.Funds;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Funds;
using Finance.Persistence;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Services;

public class FundService(
    IDispatcher<FinanceDispatchContext> dispatcher,
    FinanceDbContext dbContext)
{
    public async Task<DataResult<Fund>> Create(CreateFundRequest request, HttpRequest? httpRequest = null)
    {
        await using var tx = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var result = await dispatcher.DispatchAsync(
                new CreateFundCommand
                {
                    BankId = request.BankId,
                    CurrencyId = request.CurrencyId,
                    TimeStamp = request.TimeStamp,
                    Amount = request.Amount,
                    DailyUse = request.DailyUse,
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
            return DataResult<Fund>.Failure(ex.Message);
        }
    }

    public async Task<DataResult<Fund>> Update(UpdateFundRequest request, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new UpdateFundCommand
            {
                Id = request.Id,
                BankId = request.BankId,
                CurrencyId = request.CurrencyId,
                TimeStamp = request.TimeStamp,
                Amount = request.Amount,
                DailyUse = request.DailyUse,
            },
            httpRequest);

    public async Task<CommandResult> Delete(DeleteFundRequest request, HttpRequest? httpRequest = null)
    {
        await using var tx = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var deleteResult = await dispatcher.DispatchAsync(
                new DeleteFundsCommand { Ids = request.Ids },
                httpRequest);

            if (!deleteResult.IsSuccess)
            {
                await tx.RollbackAsync();
                return deleteResult;
            }

            foreach (var id in request.Ids)
            {
                await dispatcher.DispatchAsync(
                    new DeleteFundOwnerCommand { EntityId = id },
                    httpRequest);
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

    public async Task<DataResult<FundPermissions>> SetOwner(Guid resourceId, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new CreateFundPermissionsCommand
            {
                ResourceId = resourceId,
                PermissionLevels = [PermissionLevelEnum.Owner],
            },
            httpRequest);

    public async Task<CommandResult> DeleteOwner(Guid resourceId, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new DeleteFundOwnerCommand { EntityId = resourceId },
            httpRequest);

    public async Task<CommandResult> Activate(ActivateFundRequest request, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new ActivateFundCommand { Ids = request.Ids },
            httpRequest);

    public async Task<CommandResult> Deactivate(DeactivateFundRequest request, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new DeactivateFundCommand { Ids = request.Ids },
            httpRequest);
}