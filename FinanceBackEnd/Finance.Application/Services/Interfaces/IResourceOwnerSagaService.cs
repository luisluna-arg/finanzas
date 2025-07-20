using Finance.Domain.Models.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Finance.Application.Services.Interfaces;

public interface IResourceOwnerSagaService<TSetResourceOwnerRequest, TDeleteResourceOwnerRequest, TResourceEntity>
    where TSetResourceOwnerRequest : ISagaRequest
    where TDeleteResourceOwnerRequest : ISagaRequest
    where TResourceEntity : IEntity?
{
    Task<(TResourceEntity result, bool success)> Set(TSetResourceOwnerRequest setRequest, IDbContextTransaction? transaction = null);
    Task<bool> Delete(TDeleteResourceOwnerRequest request, IDbContextTransaction? transaction = null);
}