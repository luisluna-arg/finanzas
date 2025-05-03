using Finance.Domain.Models.Base;
using Finance.Application.Repositories;
using Finance.Persistance;
using Finance.Application.Services;
using FluentValidation;
using MediatR;

namespace Finance.Application.Commands.CreditCards;

public class BaseDeleteCommandHandler<TEntity, TId>(IEntityService<TEntity, TId> service)
    : IRequestHandler<BaseDeleteCommand<TId>>
    where TEntity : Entity<TId>
{
    protected IEntityService<TEntity, TId> Service { get => service; }

    public virtual async Task Handle(BaseDeleteCommand<TId> request, CancellationToken cancellationToken)
        => await Service.DeleteAsync(request.Ids, cancellationToken);
}

public class BaseDeleteCommand<TId> : IRequest
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
