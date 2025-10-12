using CQRSDispatch.Interfaces;
using Finance.Persistence;
using Finance.Domain.Models;
using Finance.Application.Commands;
using Finance.Application.Queries.Users;
using Finance.Application.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Finance.Application.Extensions;
using Finance.Application.Commands.Users;
using Finance.Api.Controllers.Requests.Identities;
using Microsoft.AspNetCore.Http;
using Finance.Application.Auth;
using CQRSDispatch;

namespace Finance.Application.Services;

public class UserService(
    IDispatcher<FinanceDispatchContext> dispatcher,
    FinanceDbContext dbContext,
    IdentityService identityService)
    : ISagaService<CreateUserSagaRequest, UpdateUserSagaRequest, DeleteUserSagaRequest, User>
{
    public FinanceDbContext _dbContext { get; } = dbContext;
    public IDispatcher<FinanceDispatchContext> _dispatcher { get; } = dispatcher;
    public IdentityService _identityService { get; } = identityService;

    public async Task<DataResult<User>> Create(CreateUserSagaRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null)
    {
        var localTransaction = transaction ?? await _dbContext.Database.BeginTransactionAsync();
        var shouldCommit = transaction == null;
        try
        {
            var userSourceIds = request.Identities.Select(i => i.SourceId).ToArray();
            var existingUserResult = await _dispatcher.DispatchQueryAsync(new GetUserBySourceIdsQuery(userSourceIds), httpRequest: httpRequest);
            if (existingUserResult.IsSuccess)
            {
                throw new Exception("User with the same source IDs already exists");
            }

            var userCommand = new CreateUserCommand
            {
                Username = request.Username,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Roles = request.Roles
            };
            var userResult = await _dispatcher.DispatchAsync(userCommand, httpRequest: httpRequest);
            if (!userResult.IsSuccess || userResult.Data == null)
            {
                throw new Exception(userResult.ErrorMessage);
            }

            foreach (var identity in request.Identities)
            {
                var identityRequest = new CreateIdentitySagaRequest(userResult.Data.Id, identity.Provider, identity.SourceId);
                var identityCreateResult = await _identityService.Create(identityRequest, localTransaction);
                if (!identityCreateResult.IsSuccess)
                {
                    throw new Exception(identityCreateResult.ErrorMessage);
                }
            }

            if (shouldCommit) await localTransaction.CommitAsync();

            return DataResult<User>.Success(userResult.Data);
        }
        catch (Exception ex)
        {
            if (shouldCommit) await localTransaction.RollbackAsync();

            return DataResult<User>.Failure(ex.GetInnerMostMessage());
        }
    }

    public async Task<DataResult<User>> Update(UpdateUserSagaRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null)
    {
        var localTransaction = transaction ?? await _dbContext.Database.BeginTransactionAsync();
        var shouldCommit = transaction == null;
        try
        {
            var userSourceIds = request.Identities.Select(i => i.SourceId).ToArray();
            var existingUserResult = await _dispatcher.DispatchQueryAsync(new GetUserBySourceIdsQuery(userSourceIds), httpRequest: httpRequest);
            if (!existingUserResult.IsSuccess)
            {
                throw new Exception("User with the same source IDs already exists");
            }

            var userCommand = new UpdateUserCommand
            {
                UserId = request.UserId,
                Username = request.Username,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Roles = request.Roles
            };
            var userResult = await _dispatcher.DispatchAsync(userCommand, httpRequest: httpRequest);
            if (!userResult.IsSuccess || userResult.Data == null)
            {
                throw new Exception("Failed to update user");
            }

            foreach (var identity in request.Identities)
            {
                var identityRequest = new UpdateIdentitySagaRequest(request.UserId, identity.Id, identity.Provider, identity.SourceId);
                var identityUpdateResult = await _identityService.Update(identityRequest, localTransaction);
                if (!identityUpdateResult.IsSuccess)
                {
                    throw new Exception(identityUpdateResult.ErrorMessage);
                }
            }

            if (shouldCommit) await localTransaction.CommitAsync();

            return DataResult<User>.Success(userResult.Data);
        }
        catch (Exception ex)
        {
            if (shouldCommit) await localTransaction.RollbackAsync();

            return DataResult<User>.Failure(ex.GetInnerMostMessage());
        }
    }

    public async Task<CommandResult> Delete(DeleteUserSagaRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null)
    {
        var localTransaction = transaction ?? await _dbContext.Database.BeginTransactionAsync();
        var shouldCommit = transaction == null;
        try
        {
            var userCommand = new DeleteUserCommand { UserId = request.UserId };
            var userResult = await _dispatcher.DispatchAsync(userCommand, httpRequest: httpRequest);
            if (!userResult.IsSuccess)
            {
                throw new Exception("Failed to delete user");
            }

            var identities = await _dispatcher.DispatchQueryAsync(new GetIdentitiesQuery { UserId = request.UserId }, httpRequest: httpRequest);

            foreach (var identity in identities.Data)
            {
                var identityRequest = new DeleteIdentitySagaRequest(request.UserId, identity.Id);
                var identityDeleteResult = await _identityService.Delete(identityRequest, localTransaction);
                if (!identityDeleteResult.IsSuccess)
                {
                    throw new Exception(identityDeleteResult.ErrorMessage);
                }
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