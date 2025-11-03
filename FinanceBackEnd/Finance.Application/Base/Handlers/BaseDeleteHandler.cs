using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Repositories;
using Finance.Application.Services;
using Finance.Domain.Models.Base;
using FluentValidation;

namespace Finance.Application.Commands.CreditCards;

public class BaseDeleteCommandHandler<TEntity, TId>(IEntityService<TEntity, TId> service)
    : ICommandHandler<BaseDeleteCommand<TId>>
    where TEntity : Entity<TId>
{
    protected IEntityService<TEntity, TId> Service { get => service; }

    public virtual async Task<CommandResult> ExecuteAsync(BaseDeleteCommand<TId> command, CancellationToken cancellationToken = default)
    {
        await Service.DeleteAsync(command.Ids, cancellationToken);
        return CommandResult.Success();
    }
}

public class BaseDeleteCommand<TId> : ICommand
{
    public TId[] Ids { get; set; } = [];
}

public abstract class BaseDeleteCommandValidator<TCommand, TEntity, TId> : AbstractValidator<TCommand>
    where TCommand : BaseDeleteCommand<TId>
    where TEntity : Entity<TId>
{
    protected BaseDeleteCommandValidator(
        IRepository<TEntity, TId> repository)
    {
        RuleFor(c => c)
            .MustAsync(async (c, cancellationToken) =>
            {
                foreach (var id in c.Ids)
                {
                    if (await repository.GetByIdAsync(id, cancellationToken) == null)
                    {
                        return false;
                    }
                }

                return true;
            })
            .WithMessage($"All ids for {typeof(TEntity).Name} must be valid");
    }
}
