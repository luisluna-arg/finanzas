using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Base;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Resources;

public abstract class BaseGetResourcePermissionsQuery<TSource, TId, TResourcePermissions> :
    IContextAwareQuery<FinanceDispatchContext, List<(User User, TResourcePermissions ResourcePermissions)>>,
    IContextAware<FinanceDispatchContext>
    where TSource : Entity<TId>, new()
    where TResourcePermissions : ResourcePermissions<TSource, TId>
    where TId : struct
{
    public FinanceDispatchContext Context { get; private set; }

    protected BaseGetResourcePermissionsQuery()
    {
        Context = new();
    }

    public virtual void SetContext(FinanceDispatchContext context)
    {
        Context = context;
    }
}

public abstract class BaseGetResourcePermissionsQueryHandler<TRequest, TSource, TId, TResourcePermissions>
    : IQueryHandler<TRequest, List<(User User, TResourcePermissions TResourcePermissions)>>
    where TSource : Entity<TId>, new()
    where TResourcePermissions : ResourcePermissions<TSource, TId>
    where TId : struct
    where TRequest : BaseGetResourcePermissionsQuery<TSource, TId, TResourcePermissions>
{
    protected FinanceDbContext DbContext { get; }

    public BaseGetResourcePermissionsQueryHandler(FinanceDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public virtual async Task<DataResult<List<(User User, TResourcePermissions TResourcePermissions)>>> ExecuteAsync(
        TRequest request, CancellationToken cancellationToken = default)
    {
        var resourcePermissions = DbContext.Set<TResourcePermissions>()
            .Include(rp => rp.User)
                .ThenInclude(u => u.Identities)
            .Include(rp => rp.Resource)
            .Where(rp => rp.User.Identities.Any(i => i.SourceId == request.Context.UserIdClaim));

        var sources = SourceQuery(request);

        var query =
            from rp in resourcePermissions
            join s in sources on rp.ResourceId equals s.Id
            select new { rp.User, rp };

        var anonList = await query.ToListAsync(cancellationToken);

        var result = anonList
            .Select(x => (x.User, x.rp))
            .ToList();

        return DataResult<List<(User, TResourcePermissions)>>.Success(result);
    }

    protected abstract IQueryable<TSource> SourceQuery(TRequest request);
}
