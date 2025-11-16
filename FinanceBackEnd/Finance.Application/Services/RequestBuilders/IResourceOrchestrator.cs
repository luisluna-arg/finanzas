using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Services.Interfaces;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Base;
using Finance.Domain.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;

namespace Finance.Application.Services.RequestBuilders;

public interface IResourceOrchestrator<
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
    void SetDispatcher(IDispatcher<FinanceDispatchContext> dispatcher);
    void SetOwnerService(
        IResourcePermissionsSagaService<
        TResourcePermissions,
        TOwnerOrchestrator,
        TSetOwnerRequest,
        TSetOwnerResult,
        TDeleteOwnerRequest,
        TDeleteResult> ownerService);

    Task<DataResult<TEntity>> OrchestrateCreation(TCreateRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null);
    Task<DataResult<TEntity>> OrchestrateUpdate(TUpdateRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null);
    Task<CommandResult> OrchestrateDelete(TDeleteRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null);
    DataResult<TEntity> FailureResult(TCreateRequest request, Exception ex);
    DataResult<TEntity> FailureResult(TUpdateRequest request, Exception ex);
    CommandResult FailureResult(TDeleteRequest request, Exception ex);
}
