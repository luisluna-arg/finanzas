using System.ComponentModel.DataAnnotations;
using Finance.Application.Base.Handlers;
using Finance.Domain.Models.Base;
using Finance.Application.Repositories;
using Finance.Persistance;
using FluentValidation;
using CQRSDispatch;
using CQRSDispatch.Interfaces;

namespace Finance.Application.Commands.CreditCards;

public abstract class BaseUpdateCommandHandler<TEntity, TId, TCommand>(
    IRepository<TEntity, TId> repository,
    FinanceDbContext db)
    : BaseCommandHandler<TCommand, TEntity>(db)
    where TEntity : Entity<TId>, new()
    where TCommand : BaseUpdateCommand<TEntity, TId>
{
    protected IRepository<TEntity, TId> Repository { get => repository; }

    public override async Task<DataResult<TEntity>> ExecuteAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        var record = await Repository.GetByIdAsync(command.Id, cancellationToken);
        if (record == null) throw new Exception($"{typeof(TEntity).Name} not found");

        record = await UpdateRecord(command, record, cancellationToken);

        await Repository.UpdateAsync(record, cancellationToken);
        await Repository.PersistAsync(cancellationToken);

        return DataResult<TEntity>.Success(record);
    }

    protected abstract Task<TEntity> UpdateRecord(TCommand command, TEntity record, CancellationToken cancellationToken);
}

public abstract class BaseUpdateCommand<TEntity, TId> : ICommand
{
    [Required]
    public TId Id { get; set; } = default!;
}

public abstract class BaseUpdateCommandValidator<TCommand, TEntity, TId> : AbstractValidator<TCommand>
    where TCommand : BaseUpdateCommand<TEntity, TId>
    where TEntity : Entity<TId>
{
    protected BaseUpdateCommandValidator(
        IRepository<TEntity, TId> repository)
    {
        RuleFor(c => c.Id)
            .MustAsync(repository.ExistsAsync)
            .WithMessage($"{typeof(TEntity).Name} must exist");
    }
}
