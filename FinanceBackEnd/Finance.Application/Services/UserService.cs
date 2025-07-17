using CQRSDispatch.Interfaces;
using Finance.Persistance;
using Finance.Domain.Models;
using Finance.Application.Commands;
using Finance.Application.Queries.Users;
using Finance.Api.Controllers.Requests;
using Finance.Application.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

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

    public async Task<(User result, bool success)> Create(CreateUserSagaRequest request, IDbContextTransaction? transaction = null)
    {
        var localTransaction = transaction ?? await _dbContext.Database.BeginTransactionAsync();
        try
        {
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
                var (createdIdentity, identitySuccess) = await _identityService.Create(identityRequest);
                if (!identitySuccess)
                {
                    throw new Exception("Failed to create identity");
                }
            }

            await localTransaction.CommitAsync();
            return (userResult.Data, true);
        }
        catch
        {
            await localTransaction.RollbackAsync();
            return (new User { Username = string.Empty, FirstName = string.Empty, LastName = string.Empty }, false);
        }
    }

    public async Task<(User result, bool success)> Update(UpdateUserSagaRequest request, IDbContextTransaction? transaction = null)
    {
        var localTransaction = transaction ?? await _dbContext.Database.BeginTransactionAsync();
        try
        {
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
                var (updatedIdentity, identitySuccess) = await _identityService.Update(identityRequest);
                if (!identitySuccess)
                {
                    throw new Exception("Failed to update identity");
                }
            }

            await localTransaction.CommitAsync();
            return (userResult.Data, true);
        }
        catch
        {
            await localTransaction.RollbackAsync();
            return (new User { Id = request.UserId, Username = string.Empty, FirstName = string.Empty, LastName = string.Empty }, false);
        }
    }

    public async Task<bool> Delete(DeleteUserSagaRequest request, IDbContextTransaction? transaction = null)
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
                var result = await _identityService.Delete(identityRequest);
                if (!result)
                {
                    throw new Exception("Failed to delete identity");
                }
            }

            await localTransaction.CommitAsync();
            return true;
        }
        catch
        {
            await localTransaction.RollbackAsync();
            return false;
        }
    }
}