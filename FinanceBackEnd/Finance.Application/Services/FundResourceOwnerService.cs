using CQRSDispatch.Interfaces;
using Finance.Persistance;
using Finance.Domain.Models;
using Microsoft.EntityFrameworkCore.Storage;
using Finance.Application.Commands;
using Finance.Application.Services.Interfaces;
using Finance.Application.Queries.Resources;
using Finance.Application.Commands.FundOwners;
using Microsoft.AspNetCore.Http;
using Finance.Application.Auth;
using CQRSDispatch;
using Finance.Application.Extensions;

namespace Finance.Application.Services;

public class FundResourceOwnerService(
    IDispatcher<FinanceDispatchContext> dispatcher,
    FinanceDbContext dbContext)
    : IResourceOwnerSagaService<SetFundOwnerSagaRequest, DeleteFundOwnerSagaRequest, FundResource>
{
    public FinanceDbContext _dbContext { get; } = dbContext;
    public IDispatcher<FinanceDispatchContext> _dispatcher { get; } = dispatcher;

    public async Task<DataResult<FundResource>> Set(SetFundOwnerSagaRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null)
    {
        var localTransaction = transaction ?? await _dbContext.Database.BeginTransactionAsync();
        var shouldCommit = transaction == null;
        try
        {
            var resourceOwnership = await _dispatcher.DispatchQueryAsync(new GetResourceOwnershipQuery(request.FundId), httpRequest: httpRequest);
            if (resourceOwnership.IsSuccess && resourceOwnership.Data.Any())
            {
                return DataResult<FundResource>.Success(resourceOwnership.Data.First().FundResource);
            }

            var createResourceResult = await _dispatcher.DispatchAsync(new CreateResourceCommand());
            if (!createResourceResult.IsSuccess || createResourceResult.Data == null)
            {
                throw new Exception(createResourceResult.ErrorMessage);
            }

            var createFundResourceCommand = new CreateFundResourceCommand
            {
                ResourceId = createResourceResult.Data.Id,
                FundId = request.FundId
            };
            var fundResourceResult = await _dispatcher.DispatchAsync(createFundResourceCommand);
            if (!fundResourceResult.IsSuccess || fundResourceResult.Data == null)
            {
                throw new Exception(fundResourceResult.ErrorMessage);
            }

            var createResourceOwnerCommand = new CreateResourceOwnerCommand
            {
                ResourceId = createResourceResult.Data.Id
            };
            var resourceOwnerResult = await _dispatcher.DispatchAsync(createResourceOwnerCommand, httpRequest: httpRequest);
            if (!resourceOwnerResult.IsSuccess || resourceOwnerResult.Data == null)
            {
                throw new Exception(resourceOwnerResult.ErrorMessage);
            }

            if (shouldCommit) await localTransaction.CommitAsync();

            return DataResult<FundResource>.Success(fundResourceResult.Data);
        }
        catch (Exception ex)
        {
            if (shouldCommit) await localTransaction.RollbackAsync();

            return DataResult<FundResource>.Failure(ex.GetInnerMostMessage());
        }
    }

    public async Task<CommandResult> Delete(DeleteFundOwnerSagaRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null)
    {
        var localTransaction = transaction ?? await _dbContext.Database.BeginTransactionAsync();
        var shouldCommit = transaction == null;
        try
        {
            var deleteFundOwnerCommand = new DeleteFundOwnerCommand
            {
                FundId = request.FundId
            };
            var createResourceResult = await _dispatcher.DispatchAsync(deleteFundOwnerCommand);

            if (!createResourceResult.IsSuccess)
            {
                throw new Exception(createResourceResult.ErrorMessage);
            }

            if (shouldCommit) await localTransaction.CommitAsync();

            return CommandResult.Success();
        }
        catch (Exception ex)
        {
            if (shouldCommit) await localTransaction.RollbackAsync();

            return CommandResult.Failure(ex.GetInnerMostMessage());
        }
    }
}