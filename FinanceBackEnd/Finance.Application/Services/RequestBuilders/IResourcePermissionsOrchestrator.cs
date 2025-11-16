using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Services.RequestBuilders;

public interface IResourcePermissionsOrchestrator<SetRequest, SetResult, DeleteRequest, DeleteResult>
    where SetRequest : ISagaRequest
    where SetResult : RequestResult, new()
    where DeleteRequest : ISagaRequest
    where DeleteResult : RequestResult, new()
{
    void SetDispatcher(IDispatcher<FinanceDispatchContext> dispatcher);
    Task<SetResult> OrchestrateSet(SetRequest request, HttpRequest? httpRequest);
    Task<DeleteResult> OrchestrateDelete(DeleteRequest request, HttpRequest? httpRequest);
    SetResult FailureResult(SetRequest request, Exception ex);
    DeleteResult FailureResult(DeleteRequest request, Exception ex);
}
