using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Legacy.Commands;
using Finance.Application.Legacy.Commands.Subscriptions;
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
                new DeleteSubscriptionCommand { Ids = request.Ids },
                httpRequest);

            if (!deleteResult.IsSuccess)
            {
                await tx.RollbackAsync();
                return deleteResult;
            }

            foreach (var id in request.Ids)
            {
                await dispatcher.DispatchAsync(
                    new DeleteSubscriptionOwnerCommand { EntityId = id },
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
}
