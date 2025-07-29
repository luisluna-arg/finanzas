using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Base.Handlers;
using Finance.Domain.Models.Base;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Commands;

public abstract class CreateEntityResourceCommand<TEntity, TId, TEntityResource> : ICommand<DataResult<TEntityResource>>
    where TEntity : Entity<TId>, new()
    where TEntityResource : EntityResource<TEntity, TId>, new()
{
    public Guid ResourceId { get; set; }
}

public abstract class CreateEntityResourceCommandHandler<TCommand, TEntity, TId, TEntityResource>(FinanceDbContext dbContext)
    : BaseCommandHandler<TCommand, TEntityResource>(dbContext)
    where TEntity : Entity<TId>, new()
    where TEntityResource : EntityResource<TEntity, TId>, new()
    where TCommand : CreateEntityResourceCommand<TEntity, TId, TEntityResource>
{
    public async override Task<DataResult<TEntityResource>> ExecuteAsync(TCommand request, CancellationToken cancellationToken = default)
    {
        var source = await QuerySource(request, cancellationToken);
        if (source == null)
        {
            return DataResult<TEntityResource>.Failure("CurrencyExchangeRate not found");
        }

        var resource = await DbContext.Resource
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(r => r.Id == request.ResourceId, cancellationToken);
        if (resource == null)
        {
            return DataResult<TEntityResource>.Failure("Resource not found");
        }

        var fundResource = new TEntityResource()
        {
            Resource = resource,
            ResourceId = request.ResourceId,
            ResourceSource = source,
            ResourceSourceId = source.Id
        };
        DbContext.Set<TEntityResource>().Add(fundResource);

        await DbContext.SaveChangesAsync(cancellationToken);

        return DataResult<TEntityResource>.Success(fundResource);
    }

    protected abstract Task<TEntity?> QuerySource(TCommand request, CancellationToken cancellationToken);
}
