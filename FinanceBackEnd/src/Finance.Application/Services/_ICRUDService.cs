using CQRSDispatch;
using Finance.Domain.Models.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Services;

public interface ICRUDService<TEntity, TId, TEntityPermissions, TCreateRequest, TUpdateRequest, TDeleteRequest>
    where TEntity : IEntity?
    where TEntityPermissions : class
    where TCreateRequest : class
    where TUpdateRequest : class
    where TDeleteRequest : class
{
    Task<DataResult<TEntity>> Create(TCreateRequest request, HttpRequest? httpRequest = null);

    Task<DataResult<TEntity>> Update(TUpdateRequest request, HttpRequest? httpRequest = null);

    Task<CommandResult> Delete(TDeleteRequest request, HttpRequest? httpRequest = null);

    Task<CommandResult> Activate(TId[] ids, HttpRequest? httpRequest = null);

    Task<CommandResult> Deactivate(TId[] ids, HttpRequest? httpRequest = null);

    Task<DataResult<TEntityPermissions>> SetOwner(TId resourceId, HttpRequest? httpRequest = null);

    Task<CommandResult> DeleteOwner(TId resourceId, HttpRequest? httpRequest = null);
}