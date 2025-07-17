using CQRSDispatch.Interfaces;
using Finance.Persistance;
using Finance.Domain.Models;
using Microsoft.EntityFrameworkCore.Storage;
using Finance.Api.Controllers.Requests;
using Finance.Application.Services.Interfaces;

namespace Finance.Application.Services;

public class IdentityService(
    IDispatcher dispatcher,
    FinanceDbContext dbContext)
    : ISagaService<CreateIdentitySagaRequest, UpdateIdentitySagaRequest, DeleteIdentitySagaRequest, Identity>
{
    public FinanceDbContext _dbContext { get; } = dbContext;
    public IDispatcher _dispatcher { get; } = dispatcher;

    public async Task<(Identity result, bool success)> Create(CreateIdentitySagaRequest request, IDbContextTransaction? transaction = null)
    {
        var localTransaction = transaction ?? await _dbContext.Database.BeginTransactionAsync();
        var shouldCommit = transaction == null;
        try
        {
            var identityResult = await _dispatcher.DispatchAsync(request);
            if (!identityResult.IsSuccess || identityResult.Data == null)
            {
                throw new Exception("Failed to create identity");
            }
            if (shouldCommit)
                await localTransaction.CommitAsync();
            return (identityResult.Data, true);
        }
        catch
        {
            if (shouldCommit)
                await localTransaction.RollbackAsync();
            return (new Identity { Provider = default, SourceId = string.Empty }, false);
        }
    }

    public async Task<(Identity result, bool success)> Update(UpdateIdentitySagaRequest request, IDbContextTransaction? transaction = null)
    {
        var localTransaction = transaction ?? await _dbContext.Database.BeginTransactionAsync();
        var shouldCommit = transaction == null;
        try
        {
            var identityResult = await _dispatcher.DispatchAsync(request);
            if (!identityResult.IsSuccess || identityResult.Data == null)
            {
                throw new Exception("Failed to update identity");
            }
            if (shouldCommit)
                await localTransaction.CommitAsync();
            return (identityResult.Data, true);
        }
        catch
        {
            if (shouldCommit)
                await localTransaction.RollbackAsync();
            return (new Identity { Id = request.IdentityId, Provider = default, SourceId = string.Empty }, false);
        }
    }

    public async Task<bool> Delete(DeleteIdentitySagaRequest request, IDbContextTransaction? transaction = null)
    {
        var localTransaction = transaction ?? await _dbContext.Database.BeginTransactionAsync();
        var shouldCommit = transaction == null;
        try
        {
            var identityResult = await _dispatcher.DispatchAsync(request);
            if (!identityResult.IsSuccess)
            {
                throw new Exception("Failed to delete identity");
            }
            if (shouldCommit)
                await localTransaction.CommitAsync();
            return true;
        }
        catch
        {
            if (shouldCommit)
                await localTransaction.RollbackAsync();
            return false;
        }
    }
}