using CQRSDispatch.Interfaces;
using Finance.Persistance;
using Finance.Domain.Models;
using Microsoft.EntityFrameworkCore.Storage;
using Finance.Application.Services.Interfaces;
using Finance.Application.Commands;
using Finance.Application.Extensions;
using Finance.Api.Controllers.Requests.Identities;
using Microsoft.AspNetCore.Http;
using Finance.Application.Auth;
using CQRSDispatch;

namespace Finance.Application.Services;

public class IdentityService(
    IDispatcher<FinanceDispatchContext> dispatcher,
    FinanceDbContext dbContext)
    : ISagaService<CreateIdentitySagaRequest, UpdateIdentitySagaRequest, DeleteIdentitySagaRequest, Identity>
{
    public FinanceDbContext _dbContext { get; } = dbContext;
    public IDispatcher<FinanceDispatchContext> _dispatcher { get; } = dispatcher;

    public async Task<DataResult<Identity>> Create(CreateIdentitySagaRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null)
    {
        var localTransaction = transaction ?? await _dbContext.Database.BeginTransactionAsync();
        var shouldCommit = transaction == null;
        try
        {
            var command = new CreateIdentityCommand
            {
                UserId = request.UserId,
                Provider = request.Provider,
                SourceId = request.SourceId
            };
            var identityResult = await _dispatcher.DispatchAsync(command);
            if (!identityResult.IsSuccess || identityResult.Data == null)
            {
                throw new Exception(identityResult.ErrorMessage);
            }

            if (shouldCommit) await localTransaction.CommitAsync();

            return DataResult<Identity>.Success(identityResult.Data);
        }
        catch (Exception ex)
        {
            if (shouldCommit) await localTransaction.RollbackAsync();

            return DataResult<Identity>.Failure(ex.GetInnerMostMessage());
        }
    }

    public async Task<DataResult<Identity>> Update(UpdateIdentitySagaRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null)
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

            if (shouldCommit) await localTransaction.CommitAsync();

            return DataResult<Identity>.Success(identityResult.Data);
        }
        catch (Exception ex)
        {
            if (shouldCommit) await localTransaction.RollbackAsync();

            return DataResult<Identity>.Failure(ex.GetInnerMostMessage());
        }
    }

    public async Task<CommandResult> Delete(DeleteIdentitySagaRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null)
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

            if (shouldCommit) await localTransaction.CommitAsync();

            return CommandResult.Success();
        }
        catch (Exception ex)
        {
            if (shouldCommit) await localTransaction.RollbackAsync();

            return CommandResult.Failure(ex.GetInnerMostMessage());
        }
    }
}