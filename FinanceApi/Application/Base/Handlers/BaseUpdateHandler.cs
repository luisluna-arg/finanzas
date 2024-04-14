using System.ComponentModel.DataAnnotations;
using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Domain.Models.Base;
using FinanceApi.Infrastructure.Repositories;
using FluentValidation;
using MediatR;

namespace FinanceApi.Application.Commands.CreditCards;

public abstract class BaseUpdateCommandHandler<TEntity, TId, TCommand>(
    IRepository<TEntity, TId> repository,
    FinanceDbContext db)
    : BaseResponseHandler<TCommand, TEntity>(db)
    where TEntity : Entity<TId>
    where TCommand : BaseUpdateCommand<TEntity, TId>
{
    protected IRepository<TEntity, TId> Repository { get => repository; }

    public override async Task<TEntity> Handle(TCommand request, CancellationToken cancellationToken)
    {
        var record = await Repository.GetByIdAsync(request.Id, cancellationToken);
        if (record == null) throw new Exception($"{typeof(TEntity).Name} not found");

        record = await UpdateRecord(request, record, cancellationToken);

        await Repository.UpdateAsync(record, cancellationToken);
        await Repository.PersistAsync(cancellationToken);

        return record;
    }

    protected abstract Task<TEntity> UpdateRecord(TCommand command, TEntity record, CancellationToken cancellationToken);
}

public abstract class BaseUpdateCommand<TEntity, TId> : IRequest<TEntity>
{
    [Required]
    public TId Id { get; set; }
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
