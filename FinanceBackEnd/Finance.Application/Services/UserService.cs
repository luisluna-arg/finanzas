using CQRSDispatch.Interfaces;
using Finance.Persistance;
using Finance.Domain.Models;
using Finance.Application.Commands;
using Finance.Application.Queries.Users;
using Finance.Application.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Finance.Application.Extensions;
using Finance.Application.Commands.Users;
using Finance.Api.Controllers.Requests.Identities;
using ExecutionContext = CQRSDispatch.ExecutionContext;

namespace Finance.Application.Services;

public class UserService(
    IDispatcher dispatcher,
    FinanceDbContext dbContext,
    IdentityService identityService)
    : ISagaService<CreateUserSagaRequest, UpdateUserSagaRequest, DeleteUserSagaRequest, User>
{
    public FinanceDbContext _dbContext { get; } = dbContext;
    public IDispatcher _dispatcher { get; } = dispatcher;
    public IdentityService _identityService { get; } = identityService;

    public async Task<(User result, bool success, string error)> Create(CreateUserSagaRequest request, IDbContextTransaction? transaction = null, ExecutionContext? executionContext = null)
    {
        var localTransaction = transaction ?? await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var userSourceIds = request.Identities.Select(i => i.SourceId).ToArray();
            var existingUserResult = await _dispatcher.DispatchQueryAsync(new GetUserBySourceIdsQuery(userSourceIds));
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
            var userResult = await _dispatcher.DispatchAsync(userCommand);
            if (!userResult.IsSuccess || userResult.Data == null)
            {
                throw new Exception("Failed to create user");
            }

            foreach (var identity in request.Identities)
            {
                var identityRequest = new CreateIdentitySagaRequest(userResult.Data.Id, identity.Provider, identity.SourceId);
                var (createdIdentity, identitySuccess, error) = await _identityService.Create(identityRequest, localTransaction);
                if (!identitySuccess)
                {
                    throw new Exception($"Failed to create identity: {error}");
                }
            }

            await localTransaction.CommitAsync();
            return (userResult.Data, true, string.Empty);
        }
        catch (Exception ex)
        {
            await localTransaction.RollbackAsync();
            return (new User { Username = string.Empty, FirstName = string.Empty, LastName = string.Empty }, false, ex.GetInnerMostMessage());
        }
    }

    public async Task<(User result, bool success, string error)> Update(UpdateUserSagaRequest request, IDbContextTransaction? transaction = null, ExecutionContext? executionContext = null)
    {
        var localTransaction = transaction ?? await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var userSourceIds = request.Identities.Select(i => i.SourceId).ToArray();
            var existingUserResult = await _dispatcher.DispatchQueryAsync(new GetUserBySourceIdsQuery(userSourceIds));
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
            var userResult = await _dispatcher.DispatchAsync(userCommand);
            if (!userResult.IsSuccess || userResult.Data == null)
            {
                throw new Exception("Failed to update user");
            }

            foreach (var identity in request.Identities)
            {
                var identityRequest = new UpdateIdentitySagaRequest(request.UserId, identity.Id, identity.Provider, identity.SourceId);
                var (updatedIdentity, identitySuccess, error) = await _identityService.Update(identityRequest, localTransaction);
                if (!identitySuccess)
                {
                    throw new Exception($"Failed to update identity: {error}");
                }
            }

            await localTransaction.CommitAsync();
            return (userResult.Data, true, string.Empty);
        }
        catch (Exception ex)
        {
            await localTransaction.RollbackAsync();
            return (new User { Id = request.UserId, Username = string.Empty, FirstName = string.Empty, LastName = string.Empty }, false, ex.GetInnerMostMessage());
        }
    }

    public async Task<(bool success, string error)> Delete(DeleteUserSagaRequest request, IDbContextTransaction? transaction = null, ExecutionContext? executionContext = null)
    {
        var localTransaction = transaction ?? await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var userCommand = new DeleteUserCommand { UserId = request.UserId };
            var userResult = await _dispatcher.DispatchAsync(userCommand);
            if (!userResult.IsSuccess)
            {
                throw new Exception("Failed to delete user");
            }

            var identities = await _dispatcher.DispatchQueryAsync(new GetIdentitiesQuery { UserId = request.UserId });

            foreach (var identity in identities.Data)
            {
                var identityRequest = new DeleteIdentitySagaRequest(request.UserId, identity.Id);
                var (result, error) = await _identityService.Delete(identityRequest, localTransaction);
                if (!result)
                {
                    throw new Exception($"Failed to delete identity: {error}");
                }
            }

            await localTransaction.CommitAsync();
            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            await localTransaction.RollbackAsync();
            return (false, ex.GetInnerMostMessage());
        }
    }
}