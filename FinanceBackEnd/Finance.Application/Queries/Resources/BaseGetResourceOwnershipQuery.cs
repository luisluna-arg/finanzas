using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Base;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Resources;

public abstract class BaseGetResourceOwnershipQuery<TSource, TId, TEntityResource> :
    IContextAwareQuery<FinanceDispatchContext, List<(Resource Resource, ResourceOwner ResourceOwner, TEntityResource EntityResource)>>,
    IContextAware<FinanceDispatchContext>
    where TSource : Entity<TId>, new()
    where TEntityResource : EntityResource<TSource, TId>
    where TId : struct
{
    public FinanceDispatchContext Context { get; private set; }

    protected BaseGetResourceOwnershipQuery()
    {
        Context = new();
    }

    public virtual void SetContext(FinanceDispatchContext context)
    {
        Context = context;
    }
}

public abstract class BaseGetResourceOwnershipQueryHandler<TRequest, TSource, TId, TEntityResource>
    : IQueryHandler<TRequest, List<(Resource Resource, ResourceOwner ResourceOwner, TEntityResource TEntityResource)>>
    where TSource : Entity<TId>, new()
    where TEntityResource : EntityResource<TSource, TId>
    where TId : struct
    where TRequest : BaseGetResourceOwnershipQuery<TSource, TId, TEntityResource>
{
    protected FinanceDbContext DbContext { get; }

    public BaseGetResourceOwnershipQueryHandler(FinanceDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public virtual async Task<DataResult<List<(Resource Resource, ResourceOwner ResourceOwner, TEntityResource TEntityResource)>>> ExecuteAsync(
        TRequest request, CancellationToken cancellationToken = default)
    {
        var owners = DbContext.ResourceOwner
            .Include(ro => ro.User)
            .Where(ro => ro.UserId == request.Context.UserInfo.Id);

        var entityResources = DbContext.Set<TEntityResource>()
            .Include(er => er.ResourceSource);

        var sources = SourceQuery(request);

        var query =
            from r in DbContext.Resource
            join o in owners on r.Id equals o.ResourceId
            join er in entityResources
                on new
                {
                    ResourceId = o.ResourceId,
                }
                equals new
                {
                    er.ResourceId,
                }
            join s in sources on er.ResourceSourceId equals s.Id
            select new { r, o, er };

        var anonList = await query.ToListAsync(cancellationToken);

        var result = anonList
            .Select(x => (x.r, x.o, x.er))
            .ToList();

        return DataResult<List<(Resource, ResourceOwner, TEntityResource)>>.Success(result);
    }

    protected abstract IQueryable<TSource> SourceQuery(TRequest request);
}
