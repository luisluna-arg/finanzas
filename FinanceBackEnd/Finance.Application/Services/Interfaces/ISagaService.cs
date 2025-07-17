using Finance.Domain.Models.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Finance.Application.Services.Interfaces;

public interface ISagaService<TCreateRequest, TUpdateRequest, TDeleteRequest, TEntity>
    where TCreateRequest : ISagaRequest
    where TUpdateRequest : ISagaRequest
    where TDeleteRequest : ISagaRequest
    where TEntity : IEntity?
{
    Task<(TEntity result, bool success)> Create(TCreateRequest request, IDbContextTransaction? transaction = null);
    Task<(TEntity result, bool success)> Update(TUpdateRequest request, IDbContextTransaction? transaction = null);
    Task<bool> Delete(TDeleteRequest request, IDbContextTransaction? transaction = null);
}