using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Domain.Models.Base;
using Finance.Application.Repositories;
using Finance.Persistance;

namespace Finance.Application.Queries.Base;

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
        => DataResult<TEntity?>.Success(await repository.GetByIdAsync(request.Id, cancellationToken));
}