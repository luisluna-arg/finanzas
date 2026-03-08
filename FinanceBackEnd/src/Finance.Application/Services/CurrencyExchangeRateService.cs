using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commands.CurrencyExchangeRates;
using Finance.Application.Services.CurrencyExchangeRates;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Currencies;
using Finance.Persistence;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Services;

public class CurrencyExchangeRateService(
    IDispatcher<FinanceDispatchContext> dispatcher,
    FinanceDbContext dbContext)
{
    public async Task<DataResult<CurrencyExchangeRate>> Create(CreateCurrencyExchangeRateRequest request, HttpRequest? httpRequest = null)
    {
        await using var tx = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var result = await dispatcher.DispatchAsync(
                new CreateCurrencyExchangeRateCommand
                {
                    BaseCurrencyId = request.BaseCurrencyId,
                    QuoteCurrencyId = request.QuoteCurrencyId,
                    BuyRate = request.BuyRate,
                    SellRate = request.SellRate,
                    TimeStamp = request.TimeStamp,
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
            return DataResult<CurrencyExchangeRate>.Failure(ex.Message);
        }
    }

    public async Task<DataResult<CurrencyExchangeRate>> Update(UpdateCurrencyExchangeRateRequest request, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new UpdateCurrencyExchangeRateCommand
            {
                Id = request.Id,
                BuyRate = request.BuyRate,
                SellRate = request.SellRate,
            });

    public async Task<CommandResult> Delete(DeleteCurrencyExchangeRateRequest request, HttpRequest? httpRequest = null)
    {
        await using var tx = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var deleteResult = await dispatcher.DispatchAsync(
                new DeleteCurrencyExchangeRatesCommand(request.Ids));

            if (!deleteResult.IsSuccess)
            {
                await tx.RollbackAsync();
                return deleteResult;
            }

            foreach (var id in request.Ids)
            {
                await dispatcher.DispatchAsync(
                    new DeleteCurrencyExchangeRateOwnerCommand { EntityId = id },
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

    public async Task<DataResult<CurrencyExchangeRatePermissions>> SetOwner(Guid resourceId, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new CreateCurrencyExchangeRatePermissionsCommand
            {
                ResourceId = resourceId,
                PermissionLevels = [PermissionLevelEnum.Owner],
            },
            httpRequest);

    public async Task<CommandResult> DeleteOwner(Guid resourceId, HttpRequest? httpRequest = null)
        => await dispatcher.DispatchAsync(
            new DeleteCurrencyExchangeRateOwnerCommand { EntityId = resourceId },
            httpRequest);
}
