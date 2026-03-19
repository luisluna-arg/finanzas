using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commands.IOLInvestments;
using Finance.Application.Services.IOLInvestments;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.IOLInvestments;
using FinanceBackEnd.Finance.Domain.Enums;
using Finance.Persistence;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Services;

public class IOLInvestmentService(
    IDispatcher<FinanceDispatchContext> dispatcher,
    FinanceDbContext dbContext)
    : ICRUDService<
        IOLInvestment,
        Guid,
        IOLInvestmentPermissions,
        CreateIOLInvestmentRequest,
        UpdateIOLInvestmentRequest,
        DeleteIOLInvestmentRequest>
{
    public async Task<DataResult<IOLInvestment>> Create(CreateIOLInvestmentRequest request, HttpRequest? httpRequest = null)
    {
        await using var tx = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var result = await dispatcher.DispatchAsync(
                new CreateIOLInvestmentCommand
                {
                    AssetSymbol = request.AssetSymbol,
                    Alarms = request.Alarms,
                    Quantity = request.Quantity,
                    Assets = request.Assets,
                    DailyVariation = request.DailyVariation,
                    LastPrice = request.LastPrice,
                    AverageBuyPrice = request.AverageBuyPrice,
                    AverageReturnPercent = request.AverageReturnPercent,
                    AverageReturn = request.AverageReturn,
                    Valued = request.Valued,
                    InvestmentAssetIOLTypeId = request.InvestmentAssetIOLTypeId,
                    CurrencyId = request.CurrencyId,
                });

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
            return DataResult<IOLInvestment>.Failure(ex.Message);
        }
    }

    public async Task<DataResult<IOLInvestment>> Update(UpdateIOLInvestmentRequest request, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new UpdateIOLInvestmentCommand
            {
                Id = request.Id,
                AssetSymbol = request.AssetSymbol,
                Alarms = request.Alarms,
                Quantity = request.Quantity,
                Assets = request.Assets,
                DailyVariation = request.DailyVariation,
                LastPrice = request.LastPrice,
                AverageBuyPrice = request.AverageBuyPrice,
                AverageReturnPercent = request.AverageReturnPercent,
                AverageReturn = request.AverageReturn,
                Valued = request.Valued,
                InvestmentAssetIOLTypeId = request.InvestmentAssetIOLTypeId,
            });

    public async Task<CommandResult> Delete(DeleteIOLInvestmentRequest request, HttpRequest? httpRequest = null)
    {
        await using var tx = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var deleteResult = await dispatcher.DispatchCommandAsync(
                new DeleteIOLInvestmentCommand { Ids = request.Ids });

            deleteResult.ThrowIfFailed($"Type {typeof(IOLInvestment).Name} delete operation failed");

            foreach (var id in request.Ids)
            {
                var ownershipDeleteResult = await dispatcher.DispatchAsync(
                    new DeleteIOLInvestmentOwnerCommand { EntityId = id },
                    httpRequest);

                ownershipDeleteResult.ThrowIfFailed($"Type {typeof(IOLInvestment).Name} owner delete operation failed");
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

    public async Task<DataResult<IOLInvestmentPermissions>> SetOwner(Guid resourceId, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new CreateIOLInvestmentPermissionsCommand
            {
                ResourceId = resourceId,
                PermissionLevels = [PermissionLevelEnum.Owner],
            },
            httpRequest);

    public async Task<CommandResult> DeleteOwner(Guid resourceId, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new DeleteIOLInvestmentOwnerCommand { EntityId = resourceId },
            httpRequest);

    public async Task<CommandResult> Activate(Guid[] ids, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new ActivateIOLInvestmentCommand { Ids = ids },
            httpRequest);

    public async Task<CommandResult> Deactivate(Guid[] ids, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new DeactivateIOLInvestmentCommand { Ids = ids },
            httpRequest);
}
