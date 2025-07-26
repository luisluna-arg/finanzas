using CQRSDispatch;
using Finance.Domain.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;

namespace Finance.Application.Services.Interfaces;

public interface ISagaService<TCreateRequest, TUpdateRequest, TDeleteRequest, TEntity>
    where TCreateRequest : ISagaRequest
    where TUpdateRequest : ISagaRequest
    where TDeleteRequest : ISagaRequest
    where TEntity : IEntity?
{
    Task<DataResult<TEntity>> Create(TCreateRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null);
    Task<DataResult<TEntity>> Update(TUpdateRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null);
    Task<CommandResult> Delete(TDeleteRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null);
}