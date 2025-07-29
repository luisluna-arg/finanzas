using CQRSDispatch;
using Finance.Application.Services.RequestBuilders;
using Finance.Domain.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;

namespace Finance.Application.Services.Interfaces;

public interface IResourceOwnerSagaService<TResourceEntity, TOrchestrator, SetRequest, SetResult, DeleteRequest, DeleteResult>
    where SetRequest : ISagaRequest
    where SetResult : RequestResult, new()
    where DeleteRequest : ISagaRequest
    where DeleteResult : RequestResult, new()
    where TResourceEntity : IEntity?
    where TOrchestrator : IResourceOwnerOrchestrator<SetRequest, SetResult, DeleteRequest, DeleteResult>, new()
{
    Task<SetResult> Set(SetRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null);
    Task<DeleteResult> Delete(DeleteRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null);
}