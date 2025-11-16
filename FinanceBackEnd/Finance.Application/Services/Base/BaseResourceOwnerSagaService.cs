using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Services.Interfaces;
using Finance.Application.Services.RequestBuilders;
using Finance.Domain.Models.Interfaces;
using Finance.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;

namespace Finance.Application.Services.Base;

public abstract class BaseResourcePermissionsSagaService<TEntity, TOrchestrator, TSetRequest, TSetResult, TDeleteRequest, TDeleteResult>
    : IResourcePermissionsSagaService<TEntity, TOrchestrator, TSetRequest, TSetResult, TDeleteRequest, TDeleteResult>
    where TSetRequest : ISagaRequest
    where TSetResult : RequestResult, new()
    where TDeleteRequest : ISagaRequest
    where TDeleteResult : RequestResult, new()
    where TEntity : IEntity?
    where TOrchestrator : IResourcePermissionsOrchestrator<TSetRequest, TSetResult, TDeleteRequest, TDeleteResult>, new()
{
    protected FinanceDbContext _dbContext { get; }
    protected IDispatcher<FinanceDispatchContext> _dispatcher { get; }
    protected IResourcePermissionsOrchestrator<TSetRequest, TSetResult, TDeleteRequest, TDeleteResult> _requestOrchestrator { get; }

    protected BaseResourcePermissionsSagaService(IDispatcher<FinanceDispatchContext> dispatcher, FinanceDbContext dbContext)
    {
        _dispatcher = dispatcher;
        _dbContext = dbContext;
        _requestOrchestrator = new TOrchestrator();
        _requestOrchestrator.SetDispatcher(_dispatcher);
    }

    public async Task<TSetResult> Set(TSetRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null)
    {
        var localTransaction = transaction ?? await _dbContext.Database.BeginTransactionAsync();
        var shouldCommit = transaction == null;
        try
        {
            var result = await _requestOrchestrator.OrchestrateSet(request, httpRequest);

            if (shouldCommit) await localTransaction.CommitAsync();

            return result;
        }
        catch (Exception ex)
        {
            if (shouldCommit) await localTransaction.RollbackAsync();

            return _requestOrchestrator.FailureResult(request, ex);
        }
    }

    public async Task<TDeleteResult> Delete(TDeleteRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null)
    {
        var localTransaction = transaction ?? await _dbContext.Database.BeginTransactionAsync();
        var shouldCommit = transaction == null;
        try
        {
            var result = await _requestOrchestrator.OrchestrateDelete(request, httpRequest);

            if (shouldCommit) await localTransaction.CommitAsync();

            return result;
        }
        catch (Exception ex)
        {
            if (shouldCommit) await localTransaction.RollbackAsync();

            return _requestOrchestrator.FailureResult(request, ex);
        }
    }
}
