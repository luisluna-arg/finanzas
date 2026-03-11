using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commands.Subscriptions;
using Finance.Application.Services.Subscriptions;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Subscriptions;
using Finance.Persistence;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Services;

public class SubscriptionService(
    IDispatcher<FinanceDispatchContext> dispatcher,
    FinanceDbContext dbContext)
    : ICRUDService<
        Subscription,
        Guid,
        SubscriptionPermissions,
        CreateSubscriptionRequest,
        UpdateSubscriptionRequest,
        DeleteSubscriptionRequest>
{
    public async Task<DataResult<Subscription>> Create(CreateSubscriptionRequest request, HttpRequest? httpRequest = null)
    {
        await using var tx = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var result = await dispatcher.DispatchAsync(
                new CreateSubscriptionCommand
                {
                    Name = request.Name,
                    Price = request.Price,
                    CurrencyId = request.CurrencyId,
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
            return DataResult<Subscription>.Failure(ex.Message);
        }
    }

    public async Task<DataResult<Subscription>> Update(UpdateSubscriptionRequest request, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new UpdateSubscriptionCommand
            {
                Id = request.Id,
                Name = request.Name,
                Price = request.Price,
                CurrencyId = request.CurrencyId,
                Frequency = request.Frequency,
            },
            httpRequest);

    public async Task<CommandResult> Delete(DeleteSubscriptionRequest request, HttpRequest? httpRequest = null)
    {
        await using var tx = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var deleteResult = await dispatcher.DispatchAsync(
                new DeleteSubscriptionCommand() { Ids = request.Ids },
                httpRequest);

            deleteResult.ThrowIfFailed($"Type {typeof(Subscription).Name} delete operation failed");

            foreach (var id in request.Ids)
            {
                var ownershipDeleteResult = await dispatcher.DispatchAsync(
                    new DeleteSubscriptionOwnerCommand { EntityId = id },
                    httpRequest);

                ownershipDeleteResult.ThrowIfFailed($"Type {typeof(Subscription).Name} owner delete operation failed");
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

    public async Task<DataResult<SubscriptionPermissions>> SetOwner(Guid resourceId, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new CreateSubscriptionPermissionsCommand
            {
                ResourceId = resourceId,
                PermissionLevels = [PermissionLevelEnum.Owner],
            },
            httpRequest);

    public async Task<CommandResult> DeleteOwner(Guid resourceId, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new DeleteSubscriptionOwnerCommand { EntityId = resourceId },
            httpRequest);

    public async Task<CommandResult> Activate(Guid[] ids, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new ActivateSubscriptionCommand { Ids = ids },
            httpRequest);

    public async Task<CommandResult> Deactivate(Guid[] ids, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new DeactivateSubscriptionCommand { Ids = ids },
            httpRequest);
}
