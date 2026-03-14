using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Commands.Users;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Base;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Commands.Base;

public abstract class DeleteEntityOwnerCommand<TEntity, TId, TResourcePermissions> : OwnerBaseCommand<CommandResult>
    where TEntity : Entity<TId>
    where TResourcePermissions : ResourcePermissions<TEntity, TId>
    where TId : struct
{
    public TId EntityId { get; set; }
}

public abstract class DeleteEntityOwnerCommandHandler<TCommand, TEntity, TId, TResourcePermissions> : ICommandHandler<TCommand, CommandResult>
    where TCommand : DeleteEntityOwnerCommand<TEntity, TId, TResourcePermissions>
    where TEntity : Entity<TId>
    where TResourcePermissions : ResourcePermissions<TEntity, TId>
    where TId : struct
{
    protected FinanceDbContext DbContext { get; }

    protected DeleteEntityOwnerCommandHandler(FinanceDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public async Task<CommandResult> ExecuteAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        var entityPermissions = await DbContext.Set<TResourcePermissions>()
            .Where(ep => ep.ResourceId.Equals(command.EntityId) && ep.UserId == command.Context.UserInfo.Id)
            .ToListAsync(cancellationToken);

        foreach (var permission in entityPermissions)
        {
            DbContext.Set<TResourcePermissions>().Remove(permission);
        }

        await DbContext.SaveChangesAsync(cancellationToken);

        return CommandResult.Success();
    }
}