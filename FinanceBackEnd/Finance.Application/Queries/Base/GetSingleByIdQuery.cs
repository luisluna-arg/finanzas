using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Base.Handlers;
using Finance.Application.Repositories;
using Finance.Domain.Models.Base;
using Finance.Domain.Models.Interfaces;
using Finance.Persistance;

namespace Finance.Application.Queries.Base;

public abstract class GetSingleByIdQuery<TEntity, TId> : IQuery<TEntity?>
    where TEntity : IEntity?
{
    public virtual TId Id { get; set; } = default!;
}

public class GetSingleByIdQueryHandler<TEntity, TId> : BaseQueryHandler<GetSingleByIdQuery<TEntity?, TId>, TEntity?>
    where TEntity : Entity<TId>
{
    protected readonly IRepository<TEntity, TId> repository;

    public GetSingleByIdQueryHandler(
        FinanceDbContext db,
        IRepository<TEntity, TId> repository)
        : base(db)
    {
        this.repository = repository;
    }

    public override async Task<DataResult<TEntity?>> ExecuteAsync(GetSingleByIdQuery<TEntity?, TId> request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity == null)
        {
            return DataResult<TEntity?>.Failure($"Entity with ID {request.Id} not found.");
        }

        return DataResult<TEntity?>.Success(entity);
    }
}