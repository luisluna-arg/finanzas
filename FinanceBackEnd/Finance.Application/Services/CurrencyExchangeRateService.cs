using CQRSDispatch.Interfaces;
using Finance.Domain.Models;
using Finance.Application.Services.Interfaces;
using Finance.Application.Services.Requests.CurrencyExchangeRates;
using Finance.Application.Commands.CurrencyExchangeRates;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Http;
using Finance.Application.Auth;
using Finance.Persistence;
using Finance.Application.Extensions;
using CQRSDispatch;
using Finance.Application.Commands.CurrencyExchangeRates.Owners;
using Finance.Application.Services.Orchestrators;

namespace Finance.Application.Services;

public class CurrencyExchangeRateService(
    IDispatcher<FinanceDispatchContext> dispatcher,
    FinanceDbContext dbContext,
    IResourceOwnerSagaService<
        CurrencyExchangeRateResource,
        CurrencyExchangeRateResourceOrchestrator,
        SetCurrencyExchangeRateOwnerSagaRequest,
        DataResult<CurrencyExchangeRateResource>,
        DeleteCurrencyExchangeRateOwnerSagaRequest,
        CommandResult> fundResourceOwnerService)
    : ISagaService<
        CreateCurrencyExchangeRateSagaRequest,
        UpdateCurrencyExchangeRateSagaRequest,
        DeleteCurrencyExchangeRateSagaRequest,
        CurrencyExchangeRate>
{
    public FinanceDbContext _dbContext = dbContext;
    private IDispatcher<FinanceDispatchContext> _dispatcher = dispatcher;
    private IResourceOwnerSagaService<
        CurrencyExchangeRateResource,
        CurrencyExchangeRateResourceOrchestrator,
        SetCurrencyExchangeRateOwnerSagaRequest,
        DataResult<CurrencyExchangeRateResource>,
        DeleteCurrencyExchangeRateOwnerSagaRequest,
        CommandResult> _fundResourceOwnerService = fundResourceOwnerService;

    public async Task<DataResult<CurrencyExchangeRate>> Create(CreateCurrencyExchangeRateSagaRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null)
    {
        var localTransaction = transaction ?? await _dbContext.Database.BeginTransactionAsync();
        var shouldCommit = transaction == null;
        try
        {
            var command = new CreateCurrencyExchangeRateCommand
            {
                BaseCurrencyId = request.BaseCurrencyId,
                QuoteCurrencyId = request.QuoteCurrencyId,
                BuyRate = request.BuyRate,
                SellRate = request.SellRate,
                TimeStamp = request.TimeStamp,
            };
            var createCurrencyExchangeRateResult = await _dispatcher.DispatchAsync(command, httpRequest);
            if (!createCurrencyExchangeRateResult.IsSuccess)
            {
                throw new Exception(createCurrencyExchangeRateResult.ErrorMessage);
            }

            var fundResourceOwnerResult = await _fundResourceOwnerService.Set(
                new SetCurrencyExchangeRateOwnerSagaRequest(createCurrencyExchangeRateResult.Data.Id),
                transaction: localTransaction,
                httpRequest: httpRequest);

            if (!fundResourceOwnerResult.IsSuccess)
            {
                throw new Exception(fundResourceOwnerResult.ErrorMessage);
            }

            if (shouldCommit) await localTransaction.CommitAsync();

            return DataResult<CurrencyExchangeRate>.Success(createCurrencyExchangeRateResult.Data);
        }
        catch (Exception ex)
        {
            if (shouldCommit) await localTransaction.RollbackAsync();

            return DataResult<CurrencyExchangeRate>.Failure(ex.GetInnerMostMessage());
        }
    }

    public async Task<DataResult<CurrencyExchangeRate>> Update(UpdateCurrencyExchangeRateSagaRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null)
    {
        var localTransaction = transaction ?? await _dbContext.Database.BeginTransactionAsync();
        var shouldCommit = transaction == null;
        try
        {
            var command = new UpdateCurrencyExchangeRateCommand
            {
                Id = request.Id,
                BuyRate = request.BuyRate,
                SellRate = request.SellRate
            };
            var result = await _dispatcher.DispatchAsync(command);

            if (shouldCommit) await localTransaction.CommitAsync();

            if (!result.IsSuccess)
            {
                throw new Exception(result.ErrorMessage);
            }

            return DataResult<CurrencyExchangeRate>.Success(result.Data);
        }
        catch (Exception ex)
        {
            if (shouldCommit) await localTransaction.RollbackAsync();

            return DataResult<CurrencyExchangeRate>.Failure(ex.GetInnerMostMessage());
        }
    }

    public async Task<CommandResult> Delete(DeleteCurrencyExchangeRateSagaRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null)
    {
        var localTransaction = transaction ?? await _dbContext.Database.BeginTransactionAsync();
        var shouldCommit = transaction == null;
        try
        {
            var command = new DeleteCurrencyExchangeRateCommand
            {
                Ids = [request.Id]
            };
            var result = await _dispatcher.DispatchAsync(command);

            if (shouldCommit) await localTransaction.CommitAsync();

            if (!result.IsSuccess)
            {
                throw new Exception(result.ErrorMessage);
            }

            return CommandResult.Success();
        }
        catch (Exception ex)
        {
            if (shouldCommit) await localTransaction.RollbackAsync();

            return CommandResult.Failure(ex.GetInnerMostMessage());
        }
    }
}