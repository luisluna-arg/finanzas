using CQRSDispatch;
using Finance.Domain.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;

namespace Finance.Application.Services.Interfaces;

public interface IResourceOwnerSagaService<TSetResourceOwnerRequest, TDeleteResourceOwnerRequest, TResourceEntity>
    where TSetResourceOwnerRequest : ISagaRequest
    where TDeleteResourceOwnerRequest : ISagaRequest
    where TResourceEntity : IEntity?
{
    Task<DataResult<TResourceEntity>> Set(TSetResourceOwnerRequest setRequest, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null);
    Task<CommandResult> Delete(TDeleteResourceOwnerRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null);
}