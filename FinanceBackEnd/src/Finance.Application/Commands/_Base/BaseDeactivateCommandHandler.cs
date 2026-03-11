using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Domain.Models.Base;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Commands.Base;

public abstract class BaseDeactivateCommandHandler<TCommand, TValidator, TEntity, TId>(FinanceDbContext dbContext) : ICommandHandler<TCommand>
    where TCommand : BatchUpdateBaseCommand<TId>
    where TValidator : BatchUpdateBaseCommandValidator<TCommand, TId>
    where TEntity : Entity<TId>
{
    protected readonly FinanceDbContext DbContext = dbContext;

    public async Task<CommandResult> ExecuteAsync(TCommand command, CancellationToken cancellationToken)
    {
        command.ThrowIfNotValid(Activator.CreateInstance<TValidator>());

        var entityIds = command.Ids.ToList();

        var entities = await DbContext.Set<TEntity>()
            .Where(ep => entityIds.Contains(ep.Id) && !ep.Deactivated)
            .ToListAsync(cancellationToken);

        foreach (var entity in entities)
        {
            entity.Deactivated = true;
        }

        await DbContext.SaveChangesAsync(cancellationToken);

        return CommandResult.Success();
    }
}
