using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Base.Handlers;
using Finance.Application.Repositories;
using Finance.Domain.Models.Base;
using Finance.Persistence;
using FluentValidation;

namespace Finance.Application.Commands.CreditCards;

public abstract class BaseCreateCommand<TEntity> : ICommand;

public abstract class BaseCreateCommandHandler<TCommand, TEntity, TId>(
    IRepository<TEntity, TId> repository,
    FinanceDbContext db)
    : BaseCommandHandler<TCommand, TEntity>(db)
    where TEntity : Entity<TId>, new()
    where TCommand : BaseCreateCommand<TEntity>
{
    protected IRepository<TEntity, TId> Repository { get => repository; }

    public override async Task<DataResult<TEntity>> ExecuteAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        var record = await BuildRecord(command, cancellationToken);

        await Repository.AddAsync(record, cancellationToken);

        return DataResult<TEntity>.Success(record);
    }

    protected abstract Task<TEntity> BuildRecord(TCommand command, CancellationToken cancellationToken);
}

public abstract class BaseCreateCommandValidator<TCommand, TEntity> : AbstractValidator<TCommand>
    where TCommand : BaseCreateCommand<TEntity>
{
    protected BaseCreateCommandValidator()
    {
    }
}
