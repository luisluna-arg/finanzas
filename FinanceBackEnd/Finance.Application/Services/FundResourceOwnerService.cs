using CQRSDispatch.Interfaces;
using Finance.Persistance;
using Finance.Domain.Models;
using Microsoft.EntityFrameworkCore.Storage;
using Finance.Application.Commands;
using Finance.Application.Services.Interfaces;
using Finance.Application.Queries.Resources;
using Finance.Application.Commands.FundOwners;

namespace Finance.Application.Services;

public class FundResourceOwnerService(
    IDispatcher dispatcher,
    FinanceDbContext dbContext)
    : IResourceOwnerSagaService<SetFundOwnerSagaRequest, DeleteFundOwnerSagaRequest, FundResource>
{
    public FinanceDbContext _dbContext { get; } = dbContext;
    public IDispatcher _dispatcher { get; } = dispatcher;

    public async Task<(FundResource result, bool success)> Set(SetFundOwnerSagaRequest request, IDbContextTransaction? transaction = null)
    {
        var localTransaction = transaction ?? await _dbContext.Database.BeginTransactionAsync();
        var shouldCommit = transaction == null;
        try
        {
            var resourceOwnership = await _dispatcher.DispatchQueryAsync(new GetResourceOwnershipQuery(request.UserId, request.FundId));
            if (resourceOwnership.IsSuccess && resourceOwnership.Data.Any())
            {
                return (resourceOwnership.Data.First().FundResource, true);
            }

            var createResourceResult = await _dispatcher.DispatchAsync(new CreateResourceCommand());
            if (!createResourceResult.IsSuccess || createResourceResult.Data == null)
            {
                throw new Exception("Failed to create resource");
            }

            var createFundResourceCommand = new CreateFundResourceCommand
            {
                ResourceId = createResourceResult.Data.Id,
                FundId = request.FundId
            };
            var fundResourceResult = await _dispatcher.DispatchAsync(createFundResourceCommand);
            if (!fundResourceResult.IsSuccess || fundResourceResult.Data == null)
            {
                throw new Exception("Failed to create fund resource");
            }

            var createResourceOwnerCommand = new CreateResourceOwnerCommand
            {
                ResourceId = createResourceResult.Data.Id,
                UserId = request.UserId
            };
            var resourceOwnerResult = await _dispatcher.DispatchAsync(createResourceOwnerCommand);
            if (!resourceOwnerResult.IsSuccess || resourceOwnerResult.Data == null)
            {
                throw new Exception("Failed to create resource owner");
            }

            if (shouldCommit)
                await localTransaction.CommitAsync();

            return (fundResourceResult.Data, true);
        }
        catch
        {
            if (shouldCommit)
                await localTransaction.RollbackAsync();
            // TODO This should return a more meaningful error
            return (FundResource.Default<FundResource>(), false);
        }
    }

    public async Task<bool> Delete(DeleteFundOwnerSagaRequest request, IDbContextTransaction? transaction = null)
    {
        var localTransaction = transaction ?? await _dbContext.Database.BeginTransactionAsync();
        var shouldCommit = transaction == null;
        try
        {
            var deleteFundOwnerCommand = new DeleteFundOwnerCommand
            {
                FundId = request.FundId,
                UserId = request.UserId
            };
            var createResourceResult = await _dispatcher.DispatchAsync(deleteFundOwnerCommand);

            if (!createResourceResult.IsSuccess)
            {
                throw new Exception("Failed to delete resource owner");
            }

            if (shouldCommit)
                await localTransaction.CommitAsync();
            return true;
        }
        catch
        {
            if (shouldCommit)
                await localTransaction.RollbackAsync();
            // TODO This should return a more meaningful error
            return false;
        }
    }
}