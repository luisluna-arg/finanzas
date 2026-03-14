using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commands.IOLInvestments;
using Finance.Application.Services.IOLInvestmentAssets;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.IOLInvestments;
using FinanceBackEnd.Finance.Domain.Enums;
using Finance.Persistence;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Services;

public class IOLInvestmentAssetService(
    IDispatcher<FinanceDispatchContext> dispatcher,
    FinanceDbContext dbContext)
    : ICRUDService<
        IOLInvestmentAsset,
        Guid,
        IOLInvestmentAssetPermissions,
        CreateIOLInvestmentAssetRequest,
        UpdateIOLInvestmentAssetRequest,
        DeleteIOLInvestmentAssetRequest>
{
    public async Task<DataResult<IOLInvestmentAsset>> Create(CreateIOLInvestmentAssetRequest request, HttpRequest? httpRequest = null)
    {
        await using var tx = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var result = await dispatcher.DispatchAsync(
                new CreateIOLInvestmentAssetCommand
                {
                    TypeId = request.TypeId,
                    CurrencyId = request.CurrencyId,
                    Symbol = request.Symbol,
                    Description = request.Description,
                });

            if (!result.IsSuccess)
            {
                await tx.RollbackAsync();
                return result;
            }

            await dispatcher.DispatchAsync(
                new CreateIOLInvestmentAssetPermissionsCommand
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
            return DataResult<IOLInvestmentAsset>.Failure(ex.Message);
        }
    }

    public async Task<DataResult<IOLInvestmentAsset>> Update(UpdateIOLInvestmentAssetRequest request, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new UpdateIOLInvestmentAssetCommand
            {
                Id = request.Id,
                TypeId = request.TypeId,
                CurrencyId = request.CurrencyId,
                Symbol = request.Symbol,
                Description = request.Description,
            });

    public async Task<CommandResult> Delete(DeleteIOLInvestmentAssetRequest request, HttpRequest? httpRequest = null)
    {
        await using var tx = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var deleteResult = await dispatcher.DispatchCommandAsync(
                new DeleteIOLInvestmentAssetCommand { Ids = request.Ids });

            deleteResult.ThrowIfFailed($"Type {typeof(IOLInvestmentAsset).Name} delete operation failed");

            foreach (var id in request.Ids)
            {
                var ownershipDeleteResult = await dispatcher.DispatchAsync(
                    new DeleteIOLInvestmentAssetOwnerCommand { EntityId = id },
                    httpRequest);

                ownershipDeleteResult.ThrowIfFailed($"Type {typeof(IOLInvestmentAsset).Name} owner delete operation failed");
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

    public async Task<DataResult<IOLInvestmentAssetPermissions>> SetOwner(Guid resourceId, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new CreateIOLInvestmentAssetPermissionsCommand
            {
                ResourceId = resourceId,
                PermissionLevels = [PermissionLevelEnum.Owner],
            },
            httpRequest);

    public async Task<CommandResult> DeleteOwner(Guid resourceId, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new DeleteIOLInvestmentAssetOwnerCommand { EntityId = resourceId },
            httpRequest);

    public async Task<CommandResult> Activate(Guid[] ids, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new ActivateIOLInvestmentAssetCommand { Ids = ids },
            httpRequest);

    public async Task<CommandResult> Deactivate(Guid[] ids, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new DeactivateIOLInvestmentAssetCommand { Ids = ids },
            httpRequest);
}
