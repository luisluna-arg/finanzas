using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commands.CreditCards;
using Finance.Application.Services.CreditCards;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.CreditCards;
using FinanceBackEnd.Finance.Domain.Enums;
using Finance.Persistence;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Services;

public class CreditCardService(
    IDispatcher<FinanceDispatchContext> dispatcher,
    FinanceDbContext dbContext)
{
    public async Task<DataResult<CreditCard>> Create(CreateCreditCardRequest request, HttpRequest? httpRequest = null)
    {
        await using var tx = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var result = await dispatcher.DispatchAsync(
                new Finance.Application.Legacy.Commands.CreditCards.CreateCreditCardCommand
                {
                    BankId = request.BankId,
                    Name = request.Name,
                    Deactivated = request.Deactivated,
                });

            if (!result.IsSuccess)
            {
                await tx.RollbackAsync();
                return result;
            }

            await dispatcher.DispatchAsync(
                new CreateCreditCardPermissionsCommand
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
            return DataResult<CreditCard>.Failure(ex.Message);
        }
    }

    public async Task<DataResult<CreditCard>> Update(UpdateCreditCardRequest request, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new Finance.Application.Legacy.Commands.CreditCards.UpdateCreditCardCommand
            {
                Id = request.Id,
                BankId = request.BankId,
                Name = request.Name,
                Deactivated = request.Deactivated,
            });

    public async Task<CommandResult> Delete(DeleteCreditCardRequest request, HttpRequest? httpRequest = null)
    {
        await using var tx = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var deleteResult = await dispatcher.DispatchCommandAsync(
                new Finance.Application.Legacy.Commands.CreditCards.DeleteCreditCardCommand { Ids = request.Ids });

            if (!deleteResult.IsSuccess)
            {
                await tx.RollbackAsync();
                return deleteResult;
            }

            foreach (var id in request.Ids)
            {
                await dispatcher.DispatchAsync(
                    new DeleteCreditCardOwnerCommand { EntityId = id },
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

    public async Task<DataResult<CreditCardPermissions>> SetOwner(Guid resourceId, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new CreateCreditCardPermissionsCommand
            {
                ResourceId = resourceId,
                PermissionLevels = [PermissionLevelEnum.Owner],
            },
            httpRequest);

    public async Task<CommandResult> DeleteOwner(Guid resourceId, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new DeleteCreditCardOwnerCommand { EntityId = resourceId },
            httpRequest);
}
