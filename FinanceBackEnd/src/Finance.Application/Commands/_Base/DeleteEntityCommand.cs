using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Repositories;
using Finance.Domain.Models.Base;

namespace Finance.Application.Commands.Base;

public abstract class DeleteEntityCommand<TId>() : BatchUpdateBaseCommand<TId>();

public abstract class DeleteEntityCommandHandler<TEntity, TKey, TCommand, TValidator>(IRepository<TEntity, TKey> repository)
    : ICommandHandler<TCommand>
    where TCommand : DeleteEntityCommand<TKey>
    where TEntity : Entity<TKey>
    where TValidator : DeleteEntityCommandValidator<TCommand, TKey>, new()
{
    protected readonly IRepository<TEntity, TKey> _repository = repository;

    public async Task<CommandResult> ExecuteAsync(TCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNotValid(new TValidator());

        foreach (var id in request.Ids)
        {
            await _repository.DeleteAsync(id, cancellationToken, false);
        }

        await _repository.PersistAsync(cancellationToken);

        return CommandResult.Success();
    }
}

public class DeleteEntityCommandValidator<TCommand, TKey> : BatchUpdateBaseCommandValidator<TCommand, TKey>
    where TCommand : DeleteEntityCommand<TKey>
{
}