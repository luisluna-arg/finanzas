using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commands.DebitOrigins;
using Finance.Application.Services.DebitOrigins;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Debits;
using Finance.Persistence;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Services;

public class DebitOriginService(
    IDispatcher<FinanceDispatchContext> dispatcher,
    FinanceDbContext dbContext)
{
    public async Task<DataResult<DebitOrigin>> Create(CreateDebitOriginRequest request, HttpRequest? httpRequest = null)
    {
        await using var tx = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var result = await dispatcher.DispatchAsync(
                new CreateDebitOriginCommand
                {
                    AppModuleId = request.AppModuleId,
                    Name = request.Name,
                    Deactivated = request.Deactivated,
                },
                httpRequest);

            if (!result.IsSuccess)
            {
                await tx.RollbackAsync();
                return result;
            }

            var permResult = await dispatcher.DispatchAsync(
                new CreateDebitOriginPermissionsCommand
                {
                    ResourceId = result.Data!.Id,
                    PermissionLevels = [PermissionLevelEnum.Owner],
                },
                httpRequest);

            if (!permResult.IsSuccess)
            {
                await tx.RollbackAsync();
                return DataResult<DebitOrigin>.Failure(permResult.ErrorMessage ?? "Permission creation failed.");
            }

            await tx.CommitAsync();
            return result;
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return DataResult<DebitOrigin>.Failure(ex.Message);
        }
    }

    public async Task<DataResult<DebitOrigin>> Update(UpdateDebitOriginRequest request, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new UpdateDebitOriginCommand
            {
                Id = request.Id,
                AppModuleId = request.AppModuleId,
                Name = request.Name,
                Deactivated = request.Deactivated,
            },
            httpRequest);

    public async Task<CommandResult> Delete(DeleteDebitOriginRequest request, HttpRequest? httpRequest = null)
    {
        await using var tx = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var deleteResult = await dispatcher.DispatchAsync(
                new DeleteDebitOriginCommand { Ids = request.Ids },
                httpRequest);

            if (!deleteResult.IsSuccess)
            {
                await tx.RollbackAsync();
                return deleteResult;
            }

            foreach (var id in request.Ids)
            {
                await dispatcher.DispatchAsync(
                    new DeleteDebitOriginOwnerCommand { EntityId = id },
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

    public async Task<DataResult<DebitOriginPermissions>> SetOwner(Guid resourceId, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new CreateDebitOriginPermissionsCommand
            {
                ResourceId = resourceId,
                PermissionLevels = [PermissionLevelEnum.Owner],
            },
            httpRequest);

    public async Task<CommandResult> DeleteOwner(Guid resourceId, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new DeleteDebitOriginOwnerCommand { EntityId = resourceId },
            httpRequest);

    public async Task<CommandResult> Activate(ActivateDebitOriginRequest request, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new ActivateDebitOriginCommand { Ids = request.Ids },
            httpRequest);

    public async Task<CommandResult> Deactivate(DeactivateDebitOriginRequest request, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new DeactivateDebitOriginCommand { Ids = request.Ids },
            httpRequest);
}
