using CQRSDispatch.Interfaces;
using Finance.Domain.Models;
using Finance.Application.Services.Interfaces;
using Finance.Application.Services.Requests.Funds;
using Finance.Application.Commands.Funds;
using Finance.Application.Commands.FundOwners;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Http;
using Finance.Application.Auth;
using Finance.Persistance;
using Finance.Application.Extensions;
using CQRSDispatch;

namespace Finance.Application.Services;

public class FundService(
    IDispatcher<FinanceDispatchContext> dispatcher,
    FinanceDbContext dbContext,
    IResourceOwnerSagaService<SetFundOwnerSagaRequest, DeleteFundOwnerSagaRequest, FundResource> fundResourceOwnerService)
    : ISagaService<
        CreateFundSagaRequest,
        UpdateFundSagaRequest,
        DeleteFundSagaRequest,
        Fund>
{
    public FinanceDbContext _dbContext { get; } = dbContext;
    private IDispatcher<FinanceDispatchContext> _dispatcher { get; } = dispatcher;
    private IResourceOwnerSagaService<SetFundOwnerSagaRequest, DeleteFundOwnerSagaRequest, FundResource> _fundResourceOwnerService { get; } = fundResourceOwnerService;

    public async Task<DataResult<Fund>> Create(CreateFundSagaRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null)
    {
        var localTransaction = transaction ?? await _dbContext.Database.BeginTransactionAsync();
        var shouldCommit = transaction == null;
        try
        {
            var command = new CreateFundCommand
            {
                BankId = request.BankId,
                CurrencyId = request.CurrencyId,
                TimeStamp = request.TimeStamp,
                Amount = request.Amount,
                DailyUse = request.DailyUse
            };
            var createFundResult = await _dispatcher.DispatchAsync(command);
            if (!createFundResult.IsSuccess)
            {
                throw new Exception(createFundResult.ErrorMessage);
            }

            var fundResourceOwnerResult = await _fundResourceOwnerService.Set(
                new SetFundOwnerSagaRequest(createFundResult.Data.Id),
                transaction: localTransaction,
                httpRequest: httpRequest);

            if (!fundResourceOwnerResult.IsSuccess)
            {
                throw new Exception(fundResourceOwnerResult.ErrorMessage);
            }

            if (shouldCommit) await localTransaction.CommitAsync();

            return DataResult<Fund>.Success(createFundResult.Data);
        }
        catch (Exception ex)
        {
            if (shouldCommit) await localTransaction.RollbackAsync();

            return DataResult<Fund>.Failure(ex.GetInnerMostMessage());
        }
    }

    public async Task<DataResult<Fund>> Update(UpdateFundSagaRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null)
    {
        var localTransaction = transaction ?? await _dbContext.Database.BeginTransactionAsync();
        var shouldCommit = transaction == null;
        try
        {
            var command = new UpdateFundCommand
            {
                Id = request.Id,
                BankId = request.BankId,
                CurrencyId = request.CurrencyId,
                TimeStamp = request.TimeStamp,
                Amount = request.Amount,
                DailyUse = request.DailyUse
            };
            var result = await _dispatcher.DispatchAsync(command);

            if (shouldCommit) await localTransaction.CommitAsync();

            if (!result.IsSuccess)
            {
                throw new Exception(result.ErrorMessage);
            }

            return DataResult<Fund>.Success(result.Data);
        }
        catch (Exception ex)
        {
            if (shouldCommit) await localTransaction.RollbackAsync();

            return DataResult<Fund>.Failure(ex.GetInnerMostMessage());
        }
    }

    public async Task<CommandResult> Delete(DeleteFundSagaRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null)
    {
        var localTransaction = transaction ?? await _dbContext.Database.BeginTransactionAsync();
        var shouldCommit = transaction == null;
        try
        {
            var command = new DeleteFundsCommand
            {
                Ids = [request.FundId]
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