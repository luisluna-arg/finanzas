using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Extensions;
using Finance.Application.Services.Interfaces;
using Finance.Application.Services.RequestBuilders;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Base;
using Finance.Domain.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;

namespace Finance.Application.Services.Base;

public abstract class BaseResourceOrchestrator<
    TEntity,
    TResourcePermissions,
    TCreateRequest,
    TUpdateRequest,
    TDeleteRequest,
    TSetOwnerRequest,
    TSetOwnerResult,
    TDeleteOwnerRequest,
    TDeleteResult,
    TOwnerOrchestrator>
    : IResourceOrchestrator<
        TEntity,
        TResourcePermissions,
        TCreateRequest,
        TUpdateRequest,
        TDeleteRequest,
        TSetOwnerRequest,
        TSetOwnerResult,
        TDeleteOwnerRequest,
        TDeleteResult,
        TOwnerOrchestrator>
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
{
    protected IDispatcher<FinanceDispatchContext>? _dispatcher;

    protected IResourcePermissionsSagaService<
        TResourcePermissions,
        TOwnerOrchestrator,
        TSetOwnerRequest,
        TSetOwnerResult,
        TDeleteOwnerRequest,
        TDeleteResult>? _ownerService;

    protected BaseResourceOrchestrator()
    {
    }

    protected IDispatcher<FinanceDispatchContext> Dispatcher { get => _dispatcher ?? throw new InvalidOperationException("Dispatcher is not set."); }

    public virtual void SetDispatcher(IDispatcher<FinanceDispatchContext> dispatcher)
    {
        _dispatcher = dispatcher;
    }

    protected IResourcePermissionsSagaService<
        TResourcePermissions,
        TOwnerOrchestrator,
        TSetOwnerRequest,
        TSetOwnerResult,
        TDeleteOwnerRequest,
        TDeleteResult> OwnerService
    { get => _ownerService ?? throw new InvalidOperationException("OwnerService is not set."); }

    public virtual void SetOwnerService(IResourcePermissionsSagaService<TResourcePermissions, TOwnerOrchestrator, TSetOwnerRequest, TSetOwnerResult, TDeleteOwnerRequest, TDeleteResult> ownerService)
    {
        _ownerService = ownerService;
    }

    public DataResult<TEntity> FailureResult(TCreateRequest request, Exception ex)
    {
        return DataResult<TEntity>.Failure(ex.GetInnerMostMessage());
    }

    public DataResult<TEntity> FailureResult(TUpdateRequest request, Exception ex)
    {
        return DataResult<TEntity>.Failure(ex.GetInnerMostMessage());
    }

    public CommandResult FailureResult(TDeleteRequest request, Exception ex)
    {
        return CommandResult.Failure(ex.GetInnerMostMessage());
    }

    public abstract Task<DataResult<TEntity>> OrchestrateCreation(TCreateRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null);

    public abstract Task<CommandResult> OrchestrateDelete(TDeleteRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null);

    public abstract Task<DataResult<TEntity>> OrchestrateUpdate(TUpdateRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null);
}
