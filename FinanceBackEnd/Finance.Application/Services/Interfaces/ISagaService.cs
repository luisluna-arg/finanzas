using Finance.Domain.Models.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Finance.Application.Services.Interfaces;

public interface ISagaService<TCreateRequest, TUpdateRequest, TDeleteRequest, TEntity>
    where TCreateRequest : ISagaRequest
    where TUpdateRequest : ISagaRequest
    where TDeleteRequest : ISagaRequest
    where TEntity : IEntity?
{
    Task<(TEntity result, bool success, string error)> Create(TCreateRequest request, IDbContextTransaction? transaction = null);
    Task<(TEntity result, bool success, string error)> Update(TUpdateRequest request, IDbContextTransaction? transaction = null);
    Task<(bool success, string error)> Delete(TDeleteRequest request, IDbContextTransaction? transaction = null);
}