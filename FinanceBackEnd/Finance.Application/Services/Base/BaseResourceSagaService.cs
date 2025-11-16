using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Services.Interfaces;
using Finance.Application.Services.RequestBuilders;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Base;
using Finance.Domain.Models.Interfaces;
using Finance.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;

namespace Finance.Application.Services.Base;

public abstract class BaseResourceSagaService<
    TEntity,
    TResourcePermissions,
    TOrchestrator,
    TOwnerOrchestrator,
    TCreateRequest,
    TUpdateRequest,
    TDeleteRequest,
    TSetOwnerRequest,
    TSetOwnerResult,
    TDeleteOwnerRequest,
    TDeleteResult>
    : ISagaService<TCreateRequest, TUpdateRequest, TDeleteRequest, TEntity>
    where TEntity : Entity<Guid>, IEntity, new()
    where TResourcePermissions : ResourcePermissions<TEntity, Guid>, new()
    where TCreateRequest : ISagaRequest
    where TUpdateRequest : ISagaRequest
    where TDeleteRequest : ISagaRequest
    where TSetOwnerRequest : ISagaRequest
    where TSetOwnerResult : RequestResult, new()
    where TDeleteOwnerRequest : ISagaRequest
    where TDeleteResult : RequestResult, new()
    where TOwnerOrchestrator : IResourcePermissionsOrchestrator<
        TSetOwnerRequest,
        TSetOwnerResult,
        TDeleteOwnerRequest,
        TDeleteResult>, new()
    where TOrchestrator : IResourceOrchestrator<
        TEntity,
        TResourcePermissions,
        TCreateRequest,
        TUpdateRequest,
        TDeleteRequest,
        TSetOwnerRequest,
        TSetOwnerResult,
        TDeleteOwnerRequest,
        TDeleteResult,
        TOwnerOrchestrator>, new()
{
    protected FinanceDbContext _dbContext { get; }
    protected IDispatcher<FinanceDispatchContext> _dispatcher { get; }
    protected IResourceOrchestrator<
        TEntity,
        TResourcePermissions,
        TCreateRequest,
        TUpdateRequest,
        TDeleteRequest,
        TSetOwnerRequest,
        TSetOwnerResult,
        TDeleteOwnerRequest,
        TDeleteResult,
        TOwnerOrchestrator
        > _requestOrchestrator
    { get; }

    protected BaseResourceSagaService(
        IDispatcher<FinanceDispatchContext> dispatcher,
        FinanceDbContext dbContext,
        IResourcePermissionsSagaService<
            TResourcePermissions,
            TOwnerOrchestrator,
            TSetOwnerRequest,
            TSetOwnerResult,
            TDeleteOwnerRequest,
            TDeleteResult> ResourcePermissionsService)
    {
        _dispatcher = dispatcher;
        _dbContext = dbContext;
        _requestOrchestrator = new TOrchestrator();
        _requestOrchestrator.SetDispatcher(_dispatcher);
        _requestOrchestrator.SetOwnerService(ResourcePermissionsService);
    }

    public async Task<DataResult<TEntity>> Create(TCreateRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null)
    {
        var localTransaction = transaction ?? await _dbContext.Database.BeginTransactionAsync();
        var shouldCommit = transaction == null;
        try
        {
            var result = await _requestOrchestrator.OrchestrateCreation(request, localTransaction, httpRequest);

            if (shouldCommit) await localTransaction.CommitAsync();

            return result;
        }
        catch (Exception ex)
        {
            if (shouldCommit) await localTransaction.RollbackAsync();

            return _requestOrchestrator.FailureResult(request, ex);
        }
    }

    public async Task<DataResult<TEntity>> Update(TUpdateRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null)
    {
        var localTransaction = transaction ?? await _dbContext.Database.BeginTransactionAsync();
        var shouldCommit = transaction == null;
        try
        {
            var result = await _requestOrchestrator.OrchestrateUpdate(request, localTransaction, httpRequest);

            if (shouldCommit) await localTransaction.CommitAsync();

            if (!result.IsSuccess)
            {
                throw new Exception(result.ErrorMessage);
            }

            return DataResult<TEntity>.Success(result.Data);
        }
        catch (Exception ex)
        {
            if (shouldCommit) await localTransaction.RollbackAsync();

            return _requestOrchestrator.FailureResult(request, ex);
        }
    }

    public async Task<CommandResult> Delete(TDeleteRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null)
    {
        var localTransaction = transaction ?? await _dbContext.Database.BeginTransactionAsync();
        var shouldCommit = transaction == null;
        try
        {
            var result = await _requestOrchestrator.OrchestrateDelete(request, localTransaction, httpRequest);

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

            return _requestOrchestrator.FailureResult(request, ex);
        }
    }
}
