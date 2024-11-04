using Finance.Application.Base.Handlers;
using Finance.Domain;
using Finance.Domain.Models.Base;
using Finance.Application.Repositories;
using Finance.Persistance;
using FluentValidation;
using MediatR;

namespace Finance.Application.Commands.CreditCards;

public abstract class BaseCreateCommandHandler<TEntity, TId, TCommand>(
    IRepository<TEntity, TId> repository,
    FinanceDbContext db)
    : BaseResponseHandler<TCommand, TEntity>(db)
    where TEntity : Entity<TId>
    where TCommand : BaseCreateCommand<TEntity>
{
    protected IRepository<TEntity, TId> Repository { get => repository; }

    public override async Task<TEntity> Handle(TCommand request, CancellationToken cancellationToken)
    {
        var record = await BuildRecord(request, cancellationToken);

        await Repository.AddAsync(record, cancellationToken);

        return record;
    }

    protected abstract Task<TEntity> BuildRecord(TCommand command, CancellationToken cancellationToken);
}

public abstract class BaseCreateCommand<TEntity> : IRequest<TEntity>
{
}

public abstract class BaseCreateCommandValidator<TCommand, TEntity> : AbstractValidator<TCommand>
    where TCommand : BaseCreateCommand<TEntity>
{
    protected BaseCreateCommandValidator()
    {
    }
}
