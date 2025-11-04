using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Extensions;
using Finance.Application.Services.Interfaces;
using Finance.Application.Services.RequestBuilders;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Services.Orchestrators;

public abstract class BaseResourceOwnerOrchestrator<SetRequest, SetResult, DeleteRequest, DeleteResult>
    : IResourceOwnerOrchestrator<SetRequest, SetResult, DeleteRequest, DeleteResult>
    where SetRequest : ISagaRequest
    where SetResult : RequestResult, new()
    where DeleteRequest : ISagaRequest
    where DeleteResult : RequestResult, new()
{
    protected IDispatcher<FinanceDispatchContext>? _dispatcher;

    protected BaseResourceOwnerOrchestrator()
    {
    }

    protected IDispatcher<FinanceDispatchContext> Dispatcher { get => _dispatcher ?? throw new InvalidOperationException("Dispatcher is not set."); }

    public void SetDispatcher(IDispatcher<FinanceDispatchContext> dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public abstract Task<SetResult> OrchestrateSet(SetRequest request, HttpRequest? httpRequest);

    public abstract Task<DeleteResult> OrchestrateDelete(DeleteRequest request, HttpRequest? httpRequest);

    public SetResult FailureResult(SetRequest request, Exception ex)
    {
        return RequestResult.Failure<SetResult>(ex.GetInnerMostMessage());
    }

    public DeleteResult FailureResult(DeleteRequest request, Exception ex)
    {
        return RequestResult.Failure<DeleteResult>(ex.GetInnerMostMessage());
    }
}
